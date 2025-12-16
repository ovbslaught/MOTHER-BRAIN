using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using runnerDotNet;
namespace runnerDotNet
{
	public partial class ViewHTMLField : ViewControl
	{
		protected static bool skipViewHTMLFieldCtor = false;
		public ViewHTMLField(dynamic _param_field, dynamic _param_container, dynamic _param_pageObject) // proxy constructor
			:base((XVar)_param_field, (XVar)_param_container, (XVar)_param_pageObject) {}

		public override XVar getPdfValue(dynamic data, dynamic _param_keylink = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			#endregion

			return MVCFunctions.runner_json_encode((XVar)(new XVar("text", this.processText((XVar)(this.refineHTMLValue((XVar)(data[this.field]))), (XVar)(keylink)), "isHtml", true)));
		}
		public override XVar showDBValue(dynamic data, dynamic _param_keylink, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic value = null;
			value = XVar.Clone(this.refineHTMLValue((XVar)(data[this.field])));
			return MVCFunctions.Concat("<table class=\"r-html-container\"><tbody><tr><td>", this.processText((XVar)(value), (XVar)(keylink)), "</td></tr></tbody></table>");
		}
		public override XVar getExportValue(dynamic data, dynamic _param_keylink = null, dynamic _param_html = null)
		{
			#region default values
			if(_param_keylink as Object == null) _param_keylink = new XVar("");
			if(_param_html as Object == null) _param_html = new XVar(false);
			#endregion

			#region pass-by-value parameters
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			return this.refineHTMLValue((XVar)(data[this.field]));
		}
		protected virtual XVar refineHTMLValue(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic flags = null;
			flags = XVar.Clone((XVar.Pack(GlobalVars.useUTF8) ? XVar.Pack("iu") : XVar.Pack("i")));
			value = XVar.Clone(MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/\\s+/", flags)), new XVar(" "), (XVar)(value)));
			return this.stripScriptTags((XVar)(value));
		}
		protected virtual XVar stripScriptTags(dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic flags = null;
			flags = XVar.Clone((XVar.Pack(GlobalVars.useUTF8) ? XVar.Pack("iu") : XVar.Pack("i")));
			return MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/<\\s*script\\s*(([^\">]+=\"[^\"]+\"\\s*)|", "([^'>]+='[^']+'\\s*)", ")*>.*<\\s*\\/script\\s*>/", flags)), new XVar(""), (XVar)(value));
		}
		protected override XVar textNeedsTruncating(dynamic _param_value, dynamic _param_cNumberOfChars)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			#endregion

			return (XVar)((XVar)((XVar)(!(XVar)(this.isUsedForFilter))  && (XVar)(!(XVar)(this.container.fullText)))  && (XVar)(XVar.Pack(0) < cNumberOfChars))  && (XVar)(cNumberOfChars * 1.200000 < MVCFunctions.runner_strlen((XVar)(value)));
		}
		public override XVar getValueHighlighted(dynamic _param_value, dynamic _param_highlightData)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic highlightData = XVar.Clone(_param_highlightData);
			#endregion

			dynamic flags = null, prefix = null, searchOpt = null, tagPattern = null;
			searchOpt = XVar.Clone(highlightData["searchOpt"]);
			if(searchOpt == "Equals")
			{
				return this.addHighlightingSpan((XVar)(value));
			}
			flags = XVar.Clone((XVar.Pack(this.useUTF8) ? XVar.Pack("iu") : XVar.Pack("i")));
			prefix = XVar.Clone((XVar.Pack(searchOpt == "Starts with") ? XVar.Pack("^") : XVar.Pack("")));
			tagPattern = XVar.Clone(MVCFunctions.Concat("/(<[^=>]+\\s*(?:(?:[^=>]+=\\s*'[^']+'\\s*)|", "(?:[^=>]+=\\s*\"[^\"]+\"\\s*)", ")*>)/iU"));
			foreach (KeyValuePair<XVar, dynamic> searchWord in highlightData["searchWords"].GetEnumerator())
			{
				dynamic pattern = null, searchWordParts = XVar.Array();
				searchWordParts = XVar.Clone(MVCFunctions.preg_split((XVar)(tagPattern), (XVar)(searchWord.Value)));
				if(MVCFunctions.count(searchWordParts) == 1)
				{
					dynamic highlighted = null, newSearchWord = null, res = null, valueArr = XVar.Array();
					res = new XVar("");
					highlighted = new XVar(false);
					newSearchWord = XVar.Clone(MVCFunctions.preg_replace(new XVar("/^.*>|<.*$/U"), new XVar(""), (XVar)(searchWord.Value)));
					pattern = XVar.Clone(MVCFunctions.Concat("/", prefix, "(", MVCFunctions.preg_quote((XVar)(newSearchWord), new XVar("/")), ")/", flags));
					valueArr = XVar.Clone(this.getSplitStringWithCapturedDelimiters((XVar)(tagPattern), (XVar)(value)));
					foreach (KeyValuePair<XVar, dynamic> item in valueArr.GetEnumerator())
					{
						dynamic replacedItem = null;
						if(XVar.Pack(!(XVar)(MVCFunctions.strlen((XVar)(item.Value)))))
						{
							continue;
						}
						if((XVar)((XVar)(item.Value[0] == "<")  || (XVar)(item.Value[MVCFunctions.strlen((XVar)(item.Value)) - 1] == ">"))  || (XVar)(highlighted))
						{
							res = MVCFunctions.Concat(res, item.Value);
							continue;
						}
						if(XVar.Pack(!(XVar)(this.hasHTMLEntities((XVar)(item.Value)))))
						{
							replacedItem = XVar.Clone(MVCFunctions.preg_replace((XVar)(pattern), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(item.Value)));
						}
						else
						{
							replacedItem = XVar.Clone(this.highlightValueWithHTMLEntities((XVar)(item.Value), (XVar)(pattern)));
						}
						if((XVar)(searchOpt == "Starts with")  && (XVar)(item.Value != replacedItem))
						{
							highlighted = new XVar(true);
						}
						res = MVCFunctions.Concat(res, replacedItem);
					}
					value = XVar.Clone(res);
					continue;
				}
				foreach (KeyValuePair<XVar, dynamic> item in searchWordParts.GetEnumerator())
				{
					if(XVar.Pack(MVCFunctions.trim((XVar)(item.Value))))
					{
						if((XVar)(item.Value[0] != "<")  && (XVar)(item.Value[MVCFunctions.strlen((XVar)(item.Value)) - 1] != ">"))
						{
							dynamic itemPattern = null, newItem = null;
							newItem = XVar.Clone(MVCFunctions.preg_replace(new XVar("/^.*>|<.*$/"), new XVar(""), (XVar)(item.Value)));
							itemPattern = XVar.Clone(MVCFunctions.preg_quote((XVar)(newItem), new XVar("/")));
							pattern = XVar.Clone(MVCFunctions.Concat("/(>[^>]*)(", itemPattern, ")([^<]*<)|^([^<>]*)(", itemPattern, ")(<)|(>)(", itemPattern, ")([^<>]*$)/U"));
							value = XVar.Clone(MVCFunctions.preg_replace((XVar)(pattern), (XVar)(MVCFunctions.Concat("$1", this.addHighlightingSpan(new XVar("$2")), "$3")), (XVar)(value)));
						}
					}
				}
			}
			return value;
		}
		protected virtual XVar removeCurrentTag(ref dynamic value, dynamic _param_tagStart, dynamic _param_tagEnd)
		{
			#region pass-by-value parameters
			dynamic tagStart = XVar.Clone(_param_tagStart);
			dynamic tagEnd = XVar.Clone(_param_tagEnd);
			#endregion

			if(XVar.Pack(0) < tagStart)
			{
				value = XVar.Clone(MVCFunctions.Concat(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(tagStart)), MVCFunctions.substr((XVar)(value), (XVar)(tagEnd + 1))));
			}
			else
			{
				value = XVar.Clone(MVCFunctions.substr((XVar)(value), (XVar)(tagEnd + 1)));
			}

			return null;
		}
		protected virtual XVar processTagsToClose(dynamic _param_tags, dynamic _param_tagName, dynamic _param_closingTag)
		{
			#region pass-by-value parameters
			dynamic tags = XVar.Clone(_param_tags);
			dynamic tagName = XVar.Clone(_param_tagName);
			dynamic closingTag = XVar.Clone(_param_closingTag);
			#endregion

			dynamic data = XVar.Array(), keys = null;
			data = XVar.Clone(new XVar("removeTag", false, "tags", tags));
			if(XVar.Pack(this.isTagSingleTon((XVar)(tagName))))
			{
				return data;
			}
			if(XVar.Pack(!(XVar)(closingTag)))
			{
				data.InitAndSetArrayItem(tagName, "tags", null);
				return data;
			}
			keys = XVar.Clone(MVCFunctions.array_keys((XVar)(tags), (XVar)(tagName)));
			if(XVar.Pack(!(XVar)(keys)))
			{
				data.InitAndSetArrayItem(true, "removeTag");
			}
			else
			{
				data.InitAndSetArrayItem(MVCFunctions.array_slice((XVar)(tags), new XVar(0), new XVar(-1)), "tags");
			}
			return data;
		}
		protected virtual XVar isHTMLEntity(dynamic _param_pos, dynamic _param_string)
		{
			#region pass-by-value parameters
			dynamic pos = XVar.Clone(_param_pos);
			dynamic var_string = XVar.Clone(_param_string);
			#endregion

			dynamic data = XVar.Array(), endityEndPos = null, suspect = null;
			endityEndPos = XVar.Clone(MVCFunctions.strpos((XVar)(var_string), new XVar(";"), (XVar)(pos)));
			if(XVar.Equals(XVar.Pack(endityEndPos), XVar.Pack(false)))
			{
				return new XVar("isHTMLEntity", false);
			}
			suspect = XVar.Clone(MVCFunctions.substr((XVar)(var_string), (XVar)(pos), (XVar)((endityEndPos - pos) + 1)));
			data = XVar.Clone(MVCFunctions.getHTMLEntityData((XVar)(suspect)));
			data.InitAndSetArrayItem(MVCFunctions.strlen((XVar)(suspect)), "htmlEntityLength");
			return data;
		}
		protected virtual XVar isComment(dynamic _param_pos, dynamic _param_string)
		{
			#region pass-by-value parameters
			dynamic pos = XVar.Clone(_param_pos);
			dynamic var_string = XVar.Clone(_param_string);
			#endregion

			dynamic commentClosePos = null;
			if(MVCFunctions.substr((XVar)(var_string), (XVar)(pos), new XVar(4)) != "<!--")
			{
				return new XVar("status", false);
			}
			commentClosePos = XVar.Clone(MVCFunctions.strpos((XVar)(var_string), new XVar("-->"), (XVar)(pos)));
			if(!XVar.Equals(XVar.Pack(commentClosePos), XVar.Pack(false)))
			{
				return new XVar("status", true, "commentLength", (commentClosePos - pos) + 3);
			}
			return new XVar("status", false);
		}
		protected virtual XVar getPocessedHTMLValueData(dynamic _param_value, dynamic _param_cNumberOfChars)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			#endregion

			dynamic attrCloseMark = null, attrValue = null, closingTag = null, data = XVar.Array(), i = null, j = null, notContentPositions = XVar.Array(), skipTagContent = null, skipTagStart = null, tag = null, tagName = null, tagNameReady = null, tagStart = null, tags = XVar.Array(), truncatedValue = null, waitForAttrValue = null;
			tag = XVar.Clone(closingTag = XVar.Clone(attrValue = XVar.Clone(tagNameReady = XVar.Clone(waitForAttrValue = XVar.Clone(skipTagContent = new XVar(false))))));
			attrCloseMark = new XVar("");
			tagName = new XVar("");
			tags = XVar.Clone(XVar.Array());
			notContentPositions = XVar.Clone(XVar.Array());
			i = XVar.Clone(j = new XVar(0));
			skipTagStart = XVar.Clone(-1);
			while((XVar)(i < MVCFunctions.strlen((XVar)(value)))  && (XVar)(j < cNumberOfChars))
			{
				if((XVar)(!(XVar)(tag))  && (XVar)(value[i] == "<"))
				{
					data = XVar.Clone(this.isComment((XVar)(i), (XVar)(value)));
					if(XVar.Pack(data["status"]))
					{
						notContentPositions.InitAndSetArrayItem(new XVar(0, i, 1, i + data["commentLength"]), null);
						i = XVar.Clone(i + data["commentLength"]);
					}
					else
					{
						tagStart = XVar.Clone(i);
						tagNameReady = new XVar(false);
						tag = new XVar(true);
						i = XVar.Clone(i + 1);
					}
					continue;
				}
				if((XVar)((XVar)((XVar)(tag)  && (XVar)(attrValue))  && (XVar)((XVar)(attrCloseMark == XVar.Pack(""))  || (XVar)(attrCloseMark == " ")))  && (XVar)(value[i] == ">"))
				{
					attrValue = new XVar(false);
				}
				if((XVar)((XVar)(tag)  && (XVar)(!(XVar)(attrValue)))  && (XVar)(value[i] == ">"))
				{
					if(XVar.Pack(this.isInvisibleTag((XVar)(tagName))))
					{
						skipTagContent = XVar.Clone(!(XVar)(closingTag));
					}
					if(XVar.Pack(skipTagContent))
					{
						skipTagStart = XVar.Clone(i);
					}
					else
					{
						if(skipTagStart != -1)
						{
							notContentPositions.InitAndSetArrayItem(new XVar(0, skipTagStart, 1, i), null);
							skipTagStart = XVar.Clone(-1);
						}
					}
					data = XVar.Clone(this.processTagsToClose((XVar)(tags), (XVar)(tagName), (XVar)(closingTag)));
					tags = XVar.Clone(data["tags"]);
					if(XVar.Pack(data["removeTag"]))
					{
						this.removeCurrentTag(ref value, (XVar)(tagStart), (XVar)(i));
						i = XVar.Clone(tagStart);
					}
					else
					{
						notContentPositions.InitAndSetArrayItem(new XVar(0, tagStart, 1, i), null);
						i = XVar.Clone(i + 1);
					}
					tag = XVar.Clone(closingTag = new XVar(false));
					tagNameReady = new XVar(true);
					tagName = new XVar("");
					continue;
				}
				if((XVar)(tag)  && (XVar)(!(XVar)(tagNameReady)))
				{
					if(value[i] == "/")
					{
						closingTag = new XVar(true);
					}
					else
					{
						if((XVar)(value[i] == " ")  && (XVar)(tagName != XVar.Pack("")))
						{
							tagNameReady = new XVar(true);
						}
						else
						{
							if(value[i] != " ")
							{
								tagName = MVCFunctions.Concat(tagName, value[i]);
							}
						}
					}
					i = XVar.Clone(i + 1);
					continue;
				}
				if((XVar)(tag)  && (XVar)(tagNameReady))
				{
					if((XVar)(value[i] == "=")  && (XVar)(!(XVar)(attrValue)))
					{
						waitForAttrValue = new XVar(true);
					}
					else
					{
						if((XVar)(waitForAttrValue)  && (XVar)(value[i] != " "))
						{
							if((XVar)(value[i] == "'")  || (XVar)(value[i] == "\""))
							{
								attrCloseMark = XVar.Clone(value[i]);
							}
							waitForAttrValue = new XVar(false);
							attrValue = new XVar(true);
						}
						else
						{
							if((XVar)(attrValue)  && (XVar)(value[i] == attrCloseMark))
							{
								attrValue = new XVar(false);
								attrCloseMark = new XVar(" ");
							}
						}
					}
					i = XVar.Clone(i + 1);
					continue;
				}
				if(XVar.Pack(skipTagContent))
				{
					i = XVar.Clone(i + 1);
					continue;
				}
				if(value[i] == "&")
				{
					data = XVar.Clone(this.isHTMLEntity((XVar)(i), (XVar)(value)));
					if(XVar.Pack(data["isHTMLEntity"]))
					{
						i = XVar.Clone(i + data["htmlEntityLength"]);
						j = XVar.Clone(j + data["entityLength"]);
						continue;
					}
				}
				if(ProjectSettings.ext() == "php")
				{
					if(XVar.Pack(this.useUTF8))
					{
						dynamic ordord = null;
						ordord = XVar.Clone(MVCFunctions.ord((XVar)(value[i])));
						if(192 == (XVar.BitwiseAnd(XVar.Pack(192), XVar.Pack(ordord))))
						{
							i = XVar.Clone(i + 1);
						}
						else
						{
							if(224 == (XVar.BitwiseAnd(XVar.Pack(224), XVar.Pack(ordord))))
							{
								i = XVar.Clone(i + 2);
							}
							else
							{
								if(240 == (XVar.BitwiseAnd(XVar.Pack(240), XVar.Pack(ordord))))
								{
									i = XVar.Clone(i + 3);
								}
								else
								{
									if(248 == (XVar.BitwiseAnd(XVar.Pack(248), XVar.Pack(ordord))))
									{
										i = XVar.Clone(i + 4);
									}
									else
									{
										if(252 == (XVar.BitwiseAnd(XVar.Pack(252), XVar.Pack(ordord))))
										{
											i = XVar.Clone(i + 5);
										}
									}
								}
							}
						}
					}
				}
				i = XVar.Clone(i + 1);
				j = XVar.Clone(j + 1);
			}
			truncatedValue = XVar.Clone(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(i)));
			tags = XVar.Clone(MVCFunctions.array_reverse((XVar)(tags)));
			foreach (KeyValuePair<XVar, dynamic> _tag in tags.GetEnumerator())
			{
				truncatedValue = MVCFunctions.Concat(truncatedValue, "</", _tag.Value, ">");
			}
			notContentPositions.InitAndSetArrayItem(new XVar(0, i, 1, MVCFunctions.strlen((XVar)(truncatedValue)) - 1), null);
			return new XVar("value", truncatedValue, "isTruncated", i < MVCFunctions.strlen((XVar)(value)), "notContentPositions", notContentPositions, "truncLength", i);
		}
		protected override XVar getShorteningTextAndMoreLink(dynamic _param_value, dynamic _param_cNumberOfChars, dynamic _param_keylink, dynamic _param_isLookup)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			dynamic keylink = XVar.Clone(_param_keylink);
			dynamic isLookup = XVar.Clone(_param_isLookup);
			#endregion

			dynamic data = XVar.Array(), isTruncated = null, processedValue = null;
			data = XVar.Clone(this.getPocessedHTMLValueData((XVar)(value), (XVar)(cNumberOfChars)));
			isTruncated = XVar.Clone(data["isTruncated"]);
			processedValue = XVar.Clone(data["value"]);
			if(XVar.Pack(this.searchHighlight))
			{
				if(XVar.Pack(isTruncated))
				{
					processedValue = XVar.Clone(this.highlightTruncatedBeforeMore((XVar)(value), (XVar)(processedValue), (XVar)(cNumberOfChars), (XVar)(data["truncLength"])));
				}
				else
				{
					processedValue = XVar.Clone(this.highlightSearchWord((XVar)(processedValue), new XVar(false), new XVar("")));
				}
			}
			if(XVar.Pack(isTruncated))
			{
				dynamic dataField = null, label = null, link = null, tName = null, var_params = null;
				tName = XVar.Clone(this.getContainer().tName);
				var_params = XVar.Clone(MVCFunctions.Concat("pagetype=", this.container.pSet._viewPage, "&table=", CommonFunctions.GetTableURL((XVar)(tName)), "&field=", MVCFunctions.RawUrlEncode((XVar)(this.field)), keylink));
				label = XVar.Clone(this.container.pSet.label((XVar)(this.field)));
				dataField = XVar.Clone(MVCFunctions.Concat("data-fieldlabel=\"", MVCFunctions.runner_htmlspecialchars((XVar)(label)), "\""));
				link = XVar.Clone(MVCFunctions.Concat(" <a href=\"javascript:void(0);\" data-gridlink data-query=\"", MVCFunctions.GetTableLink(new XVar("fulltext")), "?", var_params, "\" ", dataField, ">", CommonFunctions.mlang_message(new XVar("MORE")), "&nbsp;...</a>"));
				processedValue = MVCFunctions.Concat(processedValue, link);
			}
			return processedValue;
		}
		protected override XVar highlightTruncatedBeforeMore(dynamic _param_value, dynamic _param_truncatedValue, dynamic _param_cNumberOfChars, dynamic _param_truncLength)
		{
			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic truncatedValue = XVar.Clone(_param_truncatedValue);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			dynamic truncLength = XVar.Clone(_param_truncLength);
			#endregion

			dynamic data = XVar.Array(), firstPos = null, firstSearchWord = null, firstSearchWordData = XVar.Array(), hasTags = null, highlightData = XVar.Array(), processedValue = null;
			highlightData = XVar.Clone(this.searchClauseObj.getSearchHighlightingData((XVar)(this.field), (XVar)(value), new XVar(false), (XVar)(XVar.Array())));
			if(XVar.Pack(!(XVar)(highlightData)))
			{
				return truncatedValue;
			}
			data = XVar.Clone(this.getPocessedHTMLValueData((XVar)(value), (XVar)(MVCFunctions.strlen((XVar)(value)))));
			processedValue = XVar.Clone(data["value"]);
			firstSearchWordData = XVar.Clone(this.getFirstSearchWordData((XVar)(highlightData["searchWords"]), (XVar)(data["notContentPositions"]), (XVar)(processedValue)));
			firstPos = XVar.Clone(firstSearchWordData["position"]);
			hasTags = XVar.Clone(firstSearchWordData["hasTags"]);
			firstSearchWord = XVar.Clone(firstSearchWordData["searchWord"]);
			if(XVar.Equals(XVar.Pack(firstPos), XVar.Pack(false)))
			{
				return truncatedValue;
			}
			if(firstPos + MVCFunctions.strlen((XVar)(firstSearchWord)) <= truncLength)
			{
				return this.getValueHighlighted((XVar)(truncatedValue), (XVar)(highlightData));
			}
			if(firstPos <= truncLength)
			{
				dynamic truncatedUnicodeLength = null, truncatedWithSearchWordUnicodeLength = null;
				truncatedUnicodeLength = XVar.Clone(MVCFunctions.runner_strlen((XVar)(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(truncLength)))));
				truncatedWithSearchWordUnicodeLength = XVar.Clone(MVCFunctions.runner_strlen((XVar)(MVCFunctions.substr((XVar)(value), new XVar(0), (XVar)(firstPos + MVCFunctions.strlen((XVar)(firstSearchWord)))))));
				data = XVar.Clone(this.getPocessedHTMLValueData((XVar)(value), (XVar)((cNumberOfChars + truncatedWithSearchWordUnicodeLength) - truncatedUnicodeLength)));
				return this.getValueHighlighted((XVar)(data["value"]), (XVar)(highlightData));
			}
			return MVCFunctions.Concat(this.getValueHighlighted((XVar)(truncatedValue), (XVar)(highlightData)), "&nbsp;&lt;...&gt;&nbsp;", this.getSearchWordHighlighted((XVar)(hasTags), (XVar)(firstSearchWord)));
		}
		protected virtual XVar getSearchWordHighlighted(dynamic _param_hasTags, dynamic _param_searchWord)
		{
			#region pass-by-value parameters
			dynamic hasTags = XVar.Clone(_param_hasTags);
			dynamic searchWord = XVar.Clone(_param_searchWord);
			#endregion

			dynamic highligntedSearchWord = null, searchWordArr = XVar.Array(), tagPattern = null;
			if(XVar.Pack(!(XVar)(hasTags)))
			{
				return this.addHighlightingSpan((XVar)(searchWord));
			}
			highligntedSearchWord = new XVar("");
			tagPattern = XVar.Clone(MVCFunctions.Concat("/(<[^=>]+\\s*(?:(?:[^=>]+=\\s*'[^']+'\\s*)|", "(?:[^=>]+=\\s*\"[^\"]+\"\\s*)", ")*>)/iU"));
			searchWordArr = XVar.Clone(this.getSplitStringWithCapturedDelimiters((XVar)(tagPattern), (XVar)(searchWord)));
			foreach (KeyValuePair<XVar, dynamic> item in searchWordArr.GetEnumerator())
			{
				dynamic newItem = null;
				if((XVar)((XVar)(MVCFunctions.trim((XVar)(item.Value)))  && (XVar)(item.Value[0] != "<"))  && (XVar)(item.Value[MVCFunctions.strlen((XVar)(item.Value)) - 1] != ">"))
				{
					dynamic itemPattern = null;
					newItem = XVar.Clone(MVCFunctions.preg_replace(new XVar("/^.*>|<.*$/"), new XVar(""), (XVar)(item.Value)));
					itemPattern = XVar.Clone(MVCFunctions.preg_quote((XVar)(newItem), new XVar("/")));
					newItem = XVar.Clone(MVCFunctions.preg_replace((XVar)(MVCFunctions.Concat("/(", itemPattern, ")/")), (XVar)(this.addHighlightingSpan(new XVar("$1"))), (XVar)(newItem)));
				}
				highligntedSearchWord = MVCFunctions.Concat(highligntedSearchWord, newItem);
			}
			return highligntedSearchWord;
		}
		protected virtual XVar getFirstSearchWordData(dynamic _param_searchWords, dynamic _param_notContentPositions, dynamic _param_value)
		{
			#region pass-by-value parameters
			dynamic searchWords = XVar.Clone(_param_searchWords);
			dynamic notContentPositions = XVar.Clone(_param_notContentPositions);
			dynamic value = XVar.Clone(_param_value);
			#endregion

			dynamic firstPos = null, firstSearchWord = null, hasTags = null, tagPattern = null;
			hasTags = new XVar(false);
			firstSearchWord = new XVar("");
			firstPos = XVar.Clone(MVCFunctions.strlen((XVar)(value)));
			tagPattern = XVar.Clone(MVCFunctions.Concat("/(<[^=>]+\\s*(?:(?:[^=>]+=\\s*'[^']+'\\s*)|", "(?:[^=>]+=\\s*\"[^\"]+\"\\s*)", ")*>)/iU"));
			foreach (KeyValuePair<XVar, dynamic> searchWord in searchWords.GetEnumerator())
			{
				dynamic matches = null, pos = null;
				pos = XVar.Clone(MVCFunctions.strpos((XVar)(value), (XVar)(searchWord.Value)));
				hasTags = new XVar(true);
				if(XVar.Pack(!(XVar)(MVCFunctions.preg_match((XVar)(tagPattern), (XVar)(searchWord.Value), (XVar)(matches)))))
				{
					while((XVar)(this.notInContent((XVar)(pos), (XVar)(notContentPositions)))  && (XVar)(!XVar.Equals(XVar.Pack(pos), XVar.Pack(false))))
					{
						pos = XVar.Clone(MVCFunctions.strpos((XVar)(value), (XVar)(searchWord.Value), (XVar)(pos + 1)));
					}
					hasTags = new XVar(false);
				}
				if((XVar)(!XVar.Equals(XVar.Pack(pos), XVar.Pack(false)))  && (XVar)(pos < firstPos))
				{
					firstPos = XVar.Clone(pos);
					firstSearchWord = XVar.Clone(searchWord.Value);
				}
			}
			return new XVar("position", (XVar.Pack(firstPos != MVCFunctions.strlen((XVar)(value))) ? XVar.Pack(firstPos) : XVar.Pack(false)), "searchWord", firstSearchWord, "hasTags", hasTags);
		}
		protected virtual XVar notInContent(dynamic _param_pos, dynamic _param_notContentPositions)
		{
			#region pass-by-value parameters
			dynamic pos = XVar.Clone(_param_pos);
			dynamic notContentPositions = XVar.Clone(_param_notContentPositions);
			#endregion

			foreach (KeyValuePair<XVar, dynamic> positions in notContentPositions.GetEnumerator())
			{
				if((XVar)(positions.Value[0] <= pos)  && (XVar)(pos <= positions.Value[1]))
				{
					return true;
				}
			}
			return false;
		}
		protected override XVar getShorteningText(dynamic _param_value, dynamic _param_cNumberOfChars, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic cNumberOfChars = XVar.Clone(_param_cNumberOfChars);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic data = XVar.Array();
			data = XVar.Clone(this.getPocessedHTMLValueData((XVar)(value), (XVar)(cNumberOfChars)));
			if(XVar.Pack(data["isTruncated"]))
			{
				return MVCFunctions.Concat(data["value"], "&nbsp...");
			}
			return data["value"];
		}
		protected override XVar getText(dynamic _param_value, dynamic _param_html = null)
		{
			#region default values
			if(_param_html as Object == null) _param_html = new XVar(true);
			#endregion

			#region pass-by-value parameters
			dynamic value = XVar.Clone(_param_value);
			dynamic html = XVar.Clone(_param_html);
			#endregion

			dynamic data = XVar.Array();
			if(XVar.Pack(!(XVar)(html)))
			{
				return value;
			}
			data = XVar.Clone(this.getPocessedHTMLValueData((XVar)(value), (XVar)(MVCFunctions.strlen((XVar)(value)))));
			value = XVar.Clone(data["value"]);
			if(XVar.Pack(this.searchHighlight))
			{
				value = XVar.Clone(this.highlightSearchWord((XVar)(value), new XVar(false), new XVar("")));
			}
			return value;
		}
		protected virtual XVar isInvisibleTag(dynamic _param_tagName)
		{
			#region pass-by-value parameters
			dynamic tagName = XVar.Clone(_param_tagName);
			#endregion

			return tagName == "style";
		}
		protected virtual XVar isTagSingleTon(dynamic _param_tagName)
		{
			#region pass-by-value parameters
			dynamic tagName = XVar.Clone(_param_tagName);
			#endregion

			dynamic singleTonTags = null;
			singleTonTags = XVar.Clone(new XVar(0, "base", 1, "br", 2, "col", 3, "command", 4, "embed", 5, "hr", 6, "img", 7, "input", 8, "link", 9, "meta", 10, "param", 11, "source"));
			if(XVar.Pack(MVCFunctions.in_array((XVar)(tagName), (XVar)(singleTonTags))))
			{
				return true;
			}
			return false;
		}
	}
}
