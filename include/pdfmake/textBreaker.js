  'use strict';

//import StyleContextStack from './StyleContextStack';

var LineBreaker = 
(function () {
  'use strict';

  var TINF_OK = 0;
  var TINF_DATA_ERROR = -3;

  function Tree() {
    this.table = new Uint16Array(16);   /* table of code length counts */
    this.trans = new Uint16Array(288);  /* code -> symbol translation table */
  }

  function Data(source, dest) {
    this.source = source;
    this.sourceIndex = 0;
    this.tag = 0;
    this.bitcount = 0;
    
    this.dest = dest;
    this.destLen = 0;
    
    this.ltree = new Tree();  /* dynamic length/symbol tree */
    this.dtree = new Tree();  /* dynamic distance tree */
  }

  /* --------------------------------------------------- *
   * -- uninitialized global data (static structures) -- *
   * --------------------------------------------------- */

  var sltree = new Tree();
  var sdtree = new Tree();

  /* extra bits and base tables for length codes */
  var length_bits = new Uint8Array(30);
  var length_base = new Uint16Array(30);

  /* extra bits and base tables for distance codes */
  var dist_bits = new Uint8Array(30);
  var dist_base = new Uint16Array(30);

  /* special ordering of code length codes */
  var clcidx = new Uint8Array([
    16, 17, 18, 0, 8, 7, 9, 6,
    10, 5, 11, 4, 12, 3, 13, 2,
    14, 1, 15
  ]);

  /* used by tinf_decode_trees, avoids allocations every call */
  var code_tree = new Tree();
  var lengths = new Uint8Array(288 + 32);

  /* ----------------------- *
   * -- utility functions -- *
   * ----------------------- */

  /* build extra bits and base tables */
  function tinf_build_bits_base(bits, base, delta, first) {
    var i, sum;

    /* build bits table */
    for (i = 0; i < delta; ++i) bits[i] = 0;
    for (i = 0; i < 30 - delta; ++i) bits[i + delta] = i / delta | 0;

    /* build base table */
    for (sum = first, i = 0; i < 30; ++i) {
      base[i] = sum;
      sum += 1 << bits[i];
    }
  }

  /* build the fixed huffman trees */
  function tinf_build_fixed_trees(lt, dt) {
    var i;

    /* build fixed length tree */
    for (i = 0; i < 7; ++i) lt.table[i] = 0;

    lt.table[7] = 24;
    lt.table[8] = 152;
    lt.table[9] = 112;

    for (i = 0; i < 24; ++i) lt.trans[i] = 256 + i;
    for (i = 0; i < 144; ++i) lt.trans[24 + i] = i;
    for (i = 0; i < 8; ++i) lt.trans[24 + 144 + i] = 280 + i;
    for (i = 0; i < 112; ++i) lt.trans[24 + 144 + 8 + i] = 144 + i;

    /* build fixed distance tree */
    for (i = 0; i < 5; ++i) dt.table[i] = 0;

    dt.table[5] = 32;

    for (i = 0; i < 32; ++i) dt.trans[i] = i;
  }

  /* given an array of code lengths, build a tree */
  var offs = new Uint16Array(16);

  function tinf_build_tree(t, lengths, off, num) {
    var i, sum;

    /* clear code length count table */
    for (i = 0; i < 16; ++i) t.table[i] = 0;

    /* scan symbol lengths, and sum code length counts */
    for (i = 0; i < num; ++i) t.table[lengths[off + i]]++;

    t.table[0] = 0;

    /* compute offset table for distribution sort */
    for (sum = 0, i = 0; i < 16; ++i) {
      offs[i] = sum;
      sum += t.table[i];
    }

    /* create code->symbol translation table (symbols sorted by code) */
    for (i = 0; i < num; ++i) {
      if (lengths[off + i]) t.trans[offs[lengths[off + i]]++] = i;
    }
  }

  /* ---------------------- *
   * -- decode functions -- *
   * ---------------------- */

  /* get one bit from source stream */
  function tinf_getbit(d) {
    /* check if tag is empty */
    if (!d.bitcount--) {
      /* load next tag */
      d.tag = d.source[d.sourceIndex++];
      d.bitcount = 7;
    }

    /* shift bit out of tag */
    var bit = d.tag & 1;
    d.tag >>>= 1;

    return bit;
  }

  /* read a num bit value from a stream and add base */
  function tinf_read_bits(d, num, base) {
    if (!num)
      return base;

    while (d.bitcount < 24) {
      d.tag |= d.source[d.sourceIndex++] << d.bitcount;
      d.bitcount += 8;
    }

    var val = d.tag & (0xffff >>> (16 - num));
    d.tag >>>= num;
    d.bitcount -= num;
    return val + base;
  }

  /* given a data stream and a tree, decode a symbol */
  function tinf_decode_symbol(d, t) {
    while (d.bitcount < 24) {
      d.tag |= d.source[d.sourceIndex++] << d.bitcount;
      d.bitcount += 8;
    }
    
    var sum = 0, cur = 0, len = 0;
    var tag = d.tag;

    /* get more bits while code value is above sum */
    do {
      cur = 2 * cur + (tag & 1);
      tag >>>= 1;
      ++len;

      sum += t.table[len];
      cur -= t.table[len];
    } while (cur >= 0);
    
    d.tag = tag;
    d.bitcount -= len;

    return t.trans[sum + cur];
  }

  /* given a data stream, decode dynamic trees from it */
  function tinf_decode_trees(d, lt, dt) {
    var hlit, hdist, hclen;
    var i, num, length;

    /* get 5 bits HLIT (257-286) */
    hlit = tinf_read_bits(d, 5, 257);

    /* get 5 bits HDIST (1-32) */
    hdist = tinf_read_bits(d, 5, 1);

    /* get 4 bits HCLEN (4-19) */
    hclen = tinf_read_bits(d, 4, 4);

    for (i = 0; i < 19; ++i) lengths[i] = 0;

    /* read code lengths for code length alphabet */
    for (i = 0; i < hclen; ++i) {
      /* get 3 bits code length (0-7) */
      var clen = tinf_read_bits(d, 3, 0);
      lengths[clcidx[i]] = clen;
    }

    /* build code length tree */
    tinf_build_tree(code_tree, lengths, 0, 19);

    /* decode code lengths for the dynamic trees */
    for (num = 0; num < hlit + hdist;) {
      var sym = tinf_decode_symbol(d, code_tree);

      switch (sym) {
        case 16:
          /* copy previous code length 3-6 times (read 2 bits) */
          var prev = lengths[num - 1];
          for (length = tinf_read_bits(d, 2, 3); length; --length) {
            lengths[num++] = prev;
          }
          break;
        case 17:
          /* repeat code length 0 for 3-10 times (read 3 bits) */
          for (length = tinf_read_bits(d, 3, 3); length; --length) {
            lengths[num++] = 0;
          }
          break;
        case 18:
          /* repeat code length 0 for 11-138 times (read 7 bits) */
          for (length = tinf_read_bits(d, 7, 11); length; --length) {
            lengths[num++] = 0;
          }
          break;
        default:
          /* values 0-15 represent the actual code lengths */
          lengths[num++] = sym;
          break;
      }
    }

    /* build dynamic trees */
    tinf_build_tree(lt, lengths, 0, hlit);
    tinf_build_tree(dt, lengths, hlit, hdist);
  }

  /* ----------------------------- *
   * -- block inflate functions -- *
   * ----------------------------- */

  /* given a stream and two trees, inflate a block of data */
  function tinf_inflate_block_data(d, lt, dt) {
    while (1) {
      var sym = tinf_decode_symbol(d, lt);

      /* check for end of block */
      if (sym === 256) {
        return TINF_OK;
      }

      if (sym < 256) {
        d.dest[d.destLen++] = sym;
      } else {
        var length, dist, offs;
        var i;

        sym -= 257;

        /* possibly get more bits from length code */
        length = tinf_read_bits(d, length_bits[sym], length_base[sym]);

        dist = tinf_decode_symbol(d, dt);

        /* possibly get more bits from distance code */
        offs = d.destLen - tinf_read_bits(d, dist_bits[dist], dist_base[dist]);

        /* copy match */
        for (i = offs; i < offs + length; ++i) {
          d.dest[d.destLen++] = d.dest[i];
        }
      }
    }
  }

  /* inflate an uncompressed block of data */
  function tinf_inflate_uncompressed_block(d) {
    var length, invlength;
    var i;
    
    /* unread from bitbuffer */
    while (d.bitcount > 8) {
      d.sourceIndex--;
      d.bitcount -= 8;
    }

    /* get length */
    length = d.source[d.sourceIndex + 1];
    length = 256 * length + d.source[d.sourceIndex];

    /* get one's complement of length */
    invlength = d.source[d.sourceIndex + 3];
    invlength = 256 * invlength + d.source[d.sourceIndex + 2];

    /* check length */
    if (length !== (~invlength & 0x0000ffff))
      return TINF_DATA_ERROR;

    d.sourceIndex += 4;

    /* copy block */
    for (i = length; i; --i)
      d.dest[d.destLen++] = d.source[d.sourceIndex++];

    /* make sure we start next block on a byte boundary */
    d.bitcount = 0;

    return TINF_OK;
  }

  /* inflate stream from source to dest */
  function tinf_uncompress(source, dest) {
    var d = new Data(source, dest);
    var bfinal, btype, res;

    do {
      /* read final block flag */
      bfinal = tinf_getbit(d);

      /* read block type (2 bits) */
      btype = tinf_read_bits(d, 2, 0);

      /* decompress block */
      switch (btype) {
        case 0:
          /* decompress uncompressed block */
          res = tinf_inflate_uncompressed_block(d);
          break;
        case 1:
          /* decompress block with fixed huffman trees */
          res = tinf_inflate_block_data(d, sltree, sdtree);
          break;
        case 2:
          /* decompress block with dynamic huffman trees */
          tinf_decode_trees(d, d.ltree, d.dtree);
          res = tinf_inflate_block_data(d, d.ltree, d.dtree);
          break;
        default:
          res = TINF_DATA_ERROR;
      }

      if (res !== TINF_OK)
        throw new Error('Data error');

    } while (!bfinal);

    if (d.destLen < d.dest.length) {
      if (typeof d.dest.slice === 'function')
        return d.dest.slice(0, d.destLen);
      else
        return d.dest.subarray(0, d.destLen);
    }
    
    return d.dest;
  }

  /* -------------------- *
   * -- initialization -- *
   * -------------------- */

  /* build fixed huffman trees */
  tinf_build_fixed_trees(sltree, sdtree);

  /* build extra bits and base tables */
  tinf_build_bits_base(length_bits, length_base, 4, 3);
  tinf_build_bits_base(dist_bits, dist_base, 2, 1);

  /* fix a special case */
  length_bits[28] = 0;
  length_base[28] = 258;

  var tinyInflate = tinf_uncompress;

  // Shift size for getting the index-1 table offset.
  var SHIFT_1 = 6 + 5;

  // Shift size for getting the index-2 table offset.
  var SHIFT_2 = 5;

  // Difference between the two shift sizes,
  // for getting an index-1 offset from an index-2 offset. 6=11-5
  var SHIFT_1_2 = SHIFT_1 - SHIFT_2;

  // Number of index-1 entries for the BMP. 32=0x20
  // This part of the index-1 table is omitted from the serialized form.
  var OMITTED_BMP_INDEX_1_LENGTH = 0x10000 >> SHIFT_1;

  // Number of entries in an index-2 block. 64=0x40
  var INDEX_2_BLOCK_LENGTH = 1 << SHIFT_1_2;

  // Mask for getting the lower bits for the in-index-2-block offset. */
  var INDEX_2_MASK = INDEX_2_BLOCK_LENGTH - 1;

  // Shift size for shifting left the index array values.
  // Increases possible data size with 16-bit index values at the cost
  // of compactability.
  // This requires data blocks to be aligned by DATA_GRANULARITY.
  var INDEX_SHIFT = 2;

  // Number of entries in a data block. 32=0x20
  var DATA_BLOCK_LENGTH = 1 << SHIFT_2;

  // Mask for getting the lower bits for the in-data-block offset.
  var DATA_MASK = DATA_BLOCK_LENGTH - 1;

  // The part of the index-2 table for U+D800..U+DBFF stores values for
  // lead surrogate code _units_ not code _points_.
  // Values for lead surrogate code _points_ are indexed with this portion of the table.
  // Length=32=0x20=0x400>>SHIFT_2. (There are 1024=0x400 lead surrogates.)
  var LSCP_INDEX_2_OFFSET = 0x10000 >> SHIFT_2;
  var LSCP_INDEX_2_LENGTH = 0x400 >> SHIFT_2;

  // Count the lengths of both BMP pieces. 2080=0x820
  var INDEX_2_BMP_LENGTH = LSCP_INDEX_2_OFFSET + LSCP_INDEX_2_LENGTH;

  // The 2-byte UTF-8 version of the index-2 table follows at offset 2080=0x820.
  // Length 32=0x20 for lead bytes C0..DF, regardless of SHIFT_2.
  var UTF8_2B_INDEX_2_OFFSET = INDEX_2_BMP_LENGTH;
  var UTF8_2B_INDEX_2_LENGTH = 0x800 >> 6;  // U+0800 is the first code point after 2-byte UTF-8

  // The index-1 table, only used for supplementary code points, at offset 2112=0x840.
  // Variable length, for code points up to highStart, where the last single-value range starts.
  // Maximum length 512=0x200=0x100000>>SHIFT_1.
  // (For 0x100000 supplementary code points U+10000..U+10ffff.)
  //
  // The part of the index-2 table for supplementary code points starts
  // after this index-1 table.
  //
  // Both the index-1 table and the following part of the index-2 table
  // are omitted completely if there is only BMP data.
  var INDEX_1_OFFSET = UTF8_2B_INDEX_2_OFFSET + UTF8_2B_INDEX_2_LENGTH;

  // The alignment size of a data block. Also the granularity for compaction.
  var DATA_GRANULARITY = 1 << INDEX_SHIFT;

  var UnicodeTrie = function( data ) {
      var isBuffer = (typeof data.readUInt32BE === 'function') && (typeof data.slice === 'function');

      if (isBuffer || data instanceof Uint8Array) {
        // read binary format
        let uncompressedLength;
        if (isBuffer) {
          this.highStart = data.readUInt32BE(0);
          this.errorValue = data.readUInt32BE(4);
          uncompressedLength = data.readUInt32BE(8);
          data = data.slice(12);
        } else {
          var view = new DataView(data.buffer);
          this.highStart = view.getUint32(0);
          this.errorValue = view.getUint32(4);
          uncompressedLength = view.getUint32(8);
          data = data.subarray(12);
        }

        // double inflate the actual trie data
        data = tinyInflate(data, new Uint8Array(uncompressedLength));
        data = tinyInflate(data, new Uint8Array(uncompressedLength));
        this.data = new Uint32Array(data.buffer);

      } else {
        // pre-parsed data
        ({ data: this.data, highStart: this.highStart, errorValue: this.errorValue } = data);
      }
  };

  UnicodeTrie.prototype.get = function(codePoint) {
      let index;
      if ((codePoint < 0) || (codePoint > 0x10ffff)) {
        return this.errorValue;
      }

      if ((codePoint < 0xd800) || ((codePoint > 0xdbff) && (codePoint <= 0xffff))) {
        // Ordinary BMP code point, excluding leading surrogates.
        // BMP uses a single level lookup.  BMP index starts at offset 0 in the index.
        // data is stored in the index array itself.
        index = (this.data[codePoint >> SHIFT_2] << INDEX_SHIFT) + (codePoint & DATA_MASK);
        return this.data[index];
      }

      if (codePoint <= 0xffff) {
        // Lead Surrogate Code Point.  A Separate index section is stored for
        // lead surrogate code units and code points.
        //   The main index has the code unit data.
        //   For this function, we need the code point data.
        index = (this.data[LSCP_INDEX_2_OFFSET + ((codePoint - 0xd800) >> SHIFT_2)] << INDEX_SHIFT) + (codePoint & DATA_MASK);
        return this.data[index];
      }

      if (codePoint < this.highStart) {
        // Supplemental code point, use two-level lookup.
        index = this.data[(INDEX_1_OFFSET - OMITTED_BMP_INDEX_1_LENGTH) + (codePoint >> SHIFT_1)];
        index = this.data[index + ((codePoint >> SHIFT_2) & INDEX_2_MASK)];
        index = (index << INDEX_SHIFT) + (codePoint & DATA_MASK);
        return this.data[index];
      }

      return this.data[this.data.length - DATA_GRANULARITY];
    };
 

  
  var unicodeTrie = UnicodeTrie;

  function createCommonjsModule(fn) {
    var module = { exports: {} };
  	return fn(module, module.exports), module.exports;
  }

  var b64 = createCommonjsModule(function (module, exports) {
  var lookup = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';
  (function (exports) {

    var Arr = (typeof Uint8Array !== 'undefined')
      ? Uint8Array
      : Array;

  	var PLUS   = '+'.charCodeAt(0);
  	var SLASH  = '/'.charCodeAt(0);
  	var NUMBER = '0'.charCodeAt(0);
  	var LOWER  = 'a'.charCodeAt(0);
  	var UPPER  = 'A'.charCodeAt(0);
  	var PLUS_URL_SAFE = '-'.charCodeAt(0);
  	var SLASH_URL_SAFE = '_'.charCodeAt(0);

  	function decode (elt) {
  		var code = elt.charCodeAt(0);
  		if (code === PLUS ||
  		    code === PLUS_URL_SAFE)
  			return 62 // '+'
  		if (code === SLASH ||
  		    code === SLASH_URL_SAFE)
  			return 63 // '/'
  		if (code < NUMBER)
  			return -1 //no match
  		if (code < NUMBER + 10)
  			return code - NUMBER + 26 + 26
  		if (code < UPPER + 26)
  			return code - UPPER
  		if (code < LOWER + 26)
  			return code - LOWER + 26
  	}

  	function b64ToByteArray (b64) {
  		var i, j, l, tmp, placeHolders, arr;

  		if (b64.length % 4 > 0) {
  			throw new Error('Invalid string. Length must be a multiple of 4')
  		}

  		// the number of equal signs (place holders)
  		// if there are two placeholders, than the two characters before it
  		// represent one byte
  		// if there is only one, then the three characters before it represent 2 bytes
  		// this is just a cheap hack to not do indexOf twice
  		var len = b64.length;
  		placeHolders = '=' === b64.charAt(len - 2) ? 2 : '=' === b64.charAt(len - 1) ? 1 : 0;

  		// base64 is 4/3 + up to two characters of the original data
  		arr = new Arr(b64.length * 3 / 4 - placeHolders);

  		// if there are placeholders, only get up to the last complete 4 chars
  		l = placeHolders > 0 ? b64.length - 4 : b64.length;

  		var L = 0;

  		function push (v) {
  			arr[L++] = v;
  		}

  		for (i = 0, j = 0; i < l; i += 4, j += 3) {
  			tmp = (decode(b64.charAt(i)) << 18) | (decode(b64.charAt(i + 1)) << 12) | (decode(b64.charAt(i + 2)) << 6) | decode(b64.charAt(i + 3));
  			push((tmp & 0xFF0000) >> 16);
  			push((tmp & 0xFF00) >> 8);
  			push(tmp & 0xFF);
  		}

  		if (placeHolders === 2) {
  			tmp = (decode(b64.charAt(i)) << 2) | (decode(b64.charAt(i + 1)) >> 4);
  			push(tmp & 0xFF);
  		} else if (placeHolders === 1) {
  			tmp = (decode(b64.charAt(i)) << 10) | (decode(b64.charAt(i + 1)) << 4) | (decode(b64.charAt(i + 2)) >> 2);
  			push((tmp >> 8) & 0xFF);
  			push(tmp & 0xFF);
  		}

  		return arr
  	}

  	function uint8ToBase64 (uint8) {
  		var i,
  			extraBytes = uint8.length % 3, // if we have 1 byte left, pad 2 bytes
  			output = "",
  			temp, length;

  		function encode (num) {
  			return lookup.charAt(num)
  		}

  		function tripletToBase64 (num) {
  			return encode(num >> 18 & 0x3F) + encode(num >> 12 & 0x3F) + encode(num >> 6 & 0x3F) + encode(num & 0x3F)
  		}

  		// go through the array every three bytes, we'll deal with trailing stuff later
  		for (i = 0, length = uint8.length - extraBytes; i < length; i += 3) {
  			temp = (uint8[i] << 16) + (uint8[i + 1] << 8) + (uint8[i + 2]);
  			output += tripletToBase64(temp);
  		}

  		// pad the end with zeros, but make sure to not forget the extra bytes
  		switch (extraBytes) {
  			case 1:
  				temp = uint8[uint8.length - 1];
  				output += encode(temp >> 2);
  				output += encode((temp << 4) & 0x3F);
  				output += '==';
  				break
  			case 2:
  				temp = (uint8[uint8.length - 2] << 8) + (uint8[uint8.length - 1]);
  				output += encode(temp >> 10);
  				output += encode((temp >> 4) & 0x3F);
  				output += encode((temp << 2) & 0x3F);
  				output += '=';
  				break
  		}

  		return output
  	}

  	exports.toByteArray = b64ToByteArray;
  	exports.fromByteArray = uint8ToBase64;
  }(exports));
  });

  // The following break classes are handled by the pair table

  var OP = 0;   // Opening punctuation
  var CL = 1;   // Closing punctuation
  var CP = 2;   // Closing parenthesis
  var QU = 3;   // Ambiguous quotation
  var GL = 4;   // Glue
  var NS$1 = 5;   // Non-starters
  var EX = 6;   // Exclamation/Interrogation
  var SY = 7;   // Symbols allowing break after
  var IS = 8;   // Infix separator
  var PR = 9;   // Prefix
  var PO = 10;  // Postfix
  var NU = 11;  // Numeric
  var AL$1 = 12;  // Alphabetic
  var HL = 13;  // Hebrew Letter
  var ID$1 = 14;  // Ideographic
  var IN = 15;  // Inseparable characters
  var HY = 16;  // Hyphen
  var BA$1 = 17;  // Break after
  var BB = 18;  // Break before
  var B2 = 19;  // Break on either side (but not pair)
  var ZW = 20;  // Zero-width space
  var CM = 21;  // Combining marks
  var WJ$1 = 22;  // Word joiner
  var H2 = 23;  // Hangul LV
  var H3 = 24;  // Hangul LVT
  var JL = 25;  // Hangul L Jamo
  var JV = 26;  // Hangul V Jamo
  var JT = 27;  // Hangul T Jamo
  var RI = 28;  // Regional Indicator

  // The following break classes are not handled by the pair table
  var AI$1 = 29;  // Ambiguous (Alphabetic or Ideograph)
  var BK$1 = 30;  // Break (mandatory)
  var CB$1 = 31;  // Contingent break
  var CJ$1 = 32;  // Conditional Japanese Starter
  var CR$1 = 33;  // Carriage return
  var LF$1 = 34;  // Line feed
  var NL$1 = 35;  // Next line
  var SA$1 = 36;  // South-East Asian
  var SG$1 = 37;  // Surrogates
  var SP$1 = 38;  // Space
  var XX$1 = 39;  // Unknown

  var classes = {
  	OP: OP,
  	CL: CL,
  	CP: CP,
  	QU: QU,
  	GL: GL,
  	NS: NS$1,
  	EX: EX,
  	SY: SY,
  	IS: IS,
  	PR: PR,
  	PO: PO,
  	NU: NU,
  	AL: AL$1,
  	HL: HL,
  	ID: ID$1,
  	IN: IN,
  	HY: HY,
  	BA: BA$1,
  	BB: BB,
  	B2: B2,
  	ZW: ZW,
  	CM: CM,
  	WJ: WJ$1,
  	H2: H2,
  	H3: H3,
  	JL: JL,
  	JV: JV,
  	JT: JT,
  	RI: RI,
  	AI: AI$1,
  	BK: BK$1,
  	CB: CB$1,
  	CJ: CJ$1,
  	CR: CR$1,
  	LF: LF$1,
  	NL: NL$1,
  	SA: SA$1,
  	SG: SG$1,
  	SP: SP$1,
  	XX: XX$1
  };

  let CI_BRK$1, CP_BRK$1, DI_BRK$1, IN_BRK$1, PR_BRK$1;
  var DI_BRK_1 = (DI_BRK$1 = 0); // Direct break opportunity
  var IN_BRK_1 = (IN_BRK$1 = 1); // Indirect break opportunity
  var CI_BRK_1 = (CI_BRK$1 = 2); // Indirect break opportunity for combining marks
  var CP_BRK_1 = (CP_BRK$1 = 3); // Prohibited break for combining marks
  var PR_BRK_1 = (PR_BRK$1 = 4); // Prohibited break
        
  // table generated from http://www.unicode.org/reports/tr14/#Table2
  var pairTable$1 = [
    [PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, CP_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, DI_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, DI_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, PR_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1],
    [IN_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, DI_BRK$1],
    [DI_BRK$1, PR_BRK$1, PR_BRK$1, IN_BRK$1, IN_BRK$1, IN_BRK$1, PR_BRK$1, PR_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1, IN_BRK$1, DI_BRK$1, DI_BRK$1, PR_BRK$1, CI_BRK$1, PR_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, DI_BRK$1, IN_BRK$1]
  ];

  var pairs = {
  	DI_BRK: DI_BRK_1,
  	IN_BRK: IN_BRK_1,
  	CI_BRK: CI_BRK_1,
  	CP_BRK: CP_BRK_1,
  	PR_BRK: PR_BRK_1,
  	pairTable: pairTable$1
  };

  var AI, AL, BA, BK, CB, CJ, CR, ID, LF, NL, NS, SA, SG, SP, WJ, XX;


	BK = classes.BK;
	CR = classes.CR;
	LF = classes.LF;
	NL = classes.NL;
	CB = classes.CB;
	BA = classes.BA;
	SP = classes.SP;
	WJ = classes.WJ;
	SP = classes.SP;
	BK = classes.BK;
	LF = classes.LF;
	NL = classes.NL;
	AI = classes.AI;
	AL = classes.AL;
	SA = classes.SA;
	SG = classes.SG;
	XX = classes.XX;
	CJ = classes.CJ;
	ID = classes.ID;
	NS = classes.NS;

var DI_BRK = pairs.DI_BRK,
    IN_BRK = pairs.IN_BRK,
    CI_BRK = pairs.CI_BRK,
    CP_BRK = pairs.CP_BRK,
    PR_BRK = pairs.PR_BRK,
    pairTable = pairs.pairTable; 
 

  var data = b64.toByteArray("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");
  
  var classTrie = new unicodeTrie(data);

  var mapClass = function(c) {
    switch (c) {
      case AI:         return AL;
      case SA: case SG: case XX: return AL;
      case CJ:         return NS;
      default:            return c;
    }
  };
      
  var mapFirst = function(c) {
    switch (c) {
      case LF: case NL: return BK;
      case CB:     return BA;
      case SP:     return WJ;
      default:        return c;
    }
  };

  var Break = function( position, required ) {
      this.position = position;    
      this.required = required || false;	
  };

  var LineBreaker = function( string ) {
      this.string = string;
      this.pos = 0;
      this.lastPos = 0;
      this.curClass = null;
      this.nextClass = null; 
  };
  
  LineBreaker.prototype.nextCodePoint = function() {
      var code = this.string.charCodeAt(this.pos++);
      var next = this.string.charCodeAt(this.pos);
    
      // If a surrogate pair
      if ((0xd800 <= code && code <= 0xdbff) && (0xdc00 <= next && next <= 0xdfff)) {
        this.pos++;
        return ((code - 0xd800) * 0x400) + (next - 0xdc00) + 0x10000;
      }
      
      return code;
  };
  
  LineBreaker.prototype.nextCharClass =  function() {
	  return mapClass(classTrie.get(this.nextCodePoint()));
  };
  
  LineBreaker.prototype.nextBreak =  function() {
      // get the first char if we're at the beginning of the string
      if (this.curClass == null) { this.curClass = mapFirst(this.nextCharClass()); }
    
      while (this.pos < this.string.length) {
        this.lastPos = this.pos;
        var lastClass = this.nextClass;
        this.nextClass = this.nextCharClass();
      
        // explicit newline
        if ((this.curClass === BK) || ((this.curClass === CR) && (this.nextClass !== LF))) {
          this.curClass = mapFirst(mapClass(this.nextClass));
          return new Break(this.lastPos, true);
        }
      
        // handle classes not handled by the pair table
        let cur;
        switch (this.nextClass) {
          case SP:         cur = this.curClass; break;
          case BK: case LF: case NL: cur = BK; break;
          case CR:         cur = CR; break;
          case CB:         cur = BA; break;
        }
        
        if (cur != null) {
          this.curClass = cur;
          if (this.nextClass === CB) { return new Break(this.lastPos); }
          continue;
        }
      
        // if not handled already, use the pair table
        let shouldBreak = false;
        switch (pairTable[this.curClass][this.nextClass]) {
          case DI_BRK: // Direct break
            shouldBreak = true;
            break;
          
          case IN_BRK: // possible indirect break
            shouldBreak = lastClass === SP;
            break;
            
          case CI_BRK:
            shouldBreak = lastClass === SP;
            if (!shouldBreak) { continue; }
            break;
            
          case CP_BRK: // prohibited for combining marks
            if (lastClass !== SP) { continue; }
            break;
        }
          
        this.curClass = this.nextClass;
        if (shouldBreak) {
          return new Break(this.lastPos);
        }
      }
      
      if (this.pos >= this.string.length) {
        if (this.lastPos < this.string.length) {
          this.lastPos = this.string.length;
          return new Break(this.string.length);
        } else {
          return null;
        }
      }	  
  };
  
 
  var linebreaker = LineBreaker;

  return linebreaker;

}());




/**
 * @param {string} text
 * @param {boolean} noWrap
 * @returns {Array}
 */
var splitWords = function( text, noWrap ) {
  var words = [];

  if (noWrap) {
    words.push({
      text: text
    });
    return words;
  }

  var breaker = new LineBreaker(text);
  var last = 0;
  var bk;

  while (bk = breaker.nextBreak()) {
    var word = text.slice(last, bk.position);

    if (bk.required || word.match(/\r?\n$|\r$/)) {
      // new line
      word = word.replace(/\r?\n$|\r$/, '');
      words.push({
        text: word,
        lineEnd: true
      });
    } else {
      words.push({
        text: word
      });
    }

    last = bk.position;
  }

  return words;
};




/**
 * @param {Array} words
 * @param {boolean} noWrap
 * @returns {?string}
 */
var getFirstWord = function( words, noWrap ) {
  var word = words[0];

  if (word === undefined) {
    return null;
  }

  if (noWrap) {
    // text was not wrapped, we need only first word
    var tmpWords = splitWords(word.text, false);

    if (tmpWords[0] === undefined) {
      return null;
    }

    word = tmpWords[0];
  }

  return word.text;
};




var getLastWord = function( words, noWrap ) {
  var word = words[words.length - 1];

  if (word === undefined) {
    return null;
  }

  if (word.lineEnd) {
    return null;
  }

  if (noWrap) {
    // text was not wrapped, we need only last word
    var tmpWords = splitWords(word.text, false);

    if (tmpWords[tmpWords.length - 1] === undefined) {
      return null;
    }

    word = tmpWords[tmpWords.length - 1];
  }

  return word.text;
};


var TextBreaker =
function () {
	function copyStyle(source, destination) {
		source = source || {};
		destination = destination || {};
		
		for (let key in source) {
			if (key != 'text' && source.hasOwnProperty(key)) {
				destination[key] = source[key];
			}
		}

		return destination;
	}	
	
	
  function TextBreaker() {};

  /**
   * @param {string|Array} texts
   * @param {StyleContextStack} styleContextStack
   * @returns {Array}
   */
  TextBreaker.prototype.getBreaks = function getBreaks(texts, styleContextStack) {
    var results = [];

    if (!Array.isArray(texts)) {
      texts = [texts];
    }

    var lastWord = null;

    for (var i = 0, l = texts.length; i < l; i++) {
      var item = texts[i];
      var style = null;
      var words = void 0;
      //var noWrap = src_StyleContextStack.getStyleProperty(item || {}, styleContextStack, 'noWrap', false); //*
	 var noWrap = false;
	  
	  
	  var itemIsObject = (item !== null) && !Array.isArray(item) && (typeof item === 'object');
	  if ( itemIsObject ) {
        if (item._textRef && item._textRef._textNodeRef.text) {
          item.text = item._textRef._textNodeRef.text;
        }

        words = splitWords(item.text, noWrap);
        style = copyStyle(item); //*
      } else {
        words = splitWords(item, noWrap);
      }

      if (lastWord && words.length) {
        var firstWord = getFirstWord(words, noWrap);
        var wrapWords = splitWords(lastWord + firstWord, false);

        if (wrapWords.length === 1) {
          results[results.length - 1].noNewLine = true;
        }
      }

      for (var i2 = 0, l2 = words.length; i2 < l2; i2++) {
        var result = {
          text: words[i2].text
        };

        if (words[i2].lineEnd) {
          result.lineEnd = true;
        }

        copyStyle(style, result);
        results.push(result);
      }

      lastWord = null;

      if (i + 1 < l) {
        lastWord = getLastWord(words, noWrap);
      }
    }

    return results;
  };

  return TextBreaker;
}();




