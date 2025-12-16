/**
 * ReCaptcha constructor
 * @class Runner.controls.ReCaptcha
 */
Runner.controls.ReCaptcha = Runner.extend( Runner.emptyFn, {
	/**
	 * Id hidden input field for after captcha valid
	 */
	inputCaptchaId: '',
	
	/**
	 * The Google ReCaptcha siteKey
	 */
	siteKey: '',

	/**
	 * @constructor
	 */
	constructor: function( cfg ) {
		Runner.apply(this, cfg);
		Runner.controls.ReCaptcha.superclass.constructor.call(this, cfg);
	},

	/**
	 * Init the page's reCaptcha 
	 * @param {object} reCaptchaData
	 * @public
	 */
	init: function() {
		var inputHiddenField = $('#' + this.inputCaptchaId),
			client_id;

		if ( typeof inputHiddenField.parent().html() != "undefined" )
		{
			$('<div id="reCaptchaFor' + this.inputCaptchaId + '"></div>').insertAfter(inputHiddenField);

			client_id = grecaptcha.render('reCaptchaFor' + this.inputCaptchaId, {
		  		'sitekey' : this.siteKey,
		  		'callback' : function(response) {
					inputHiddenField.val(response);
				},
			});

			$('#reCaptchaFor' + this.inputCaptchaId).attr("data-client", client_id);
		}
	},

});

/**
 * Global ReCaptchaLoader
 * @constructor
 */
Runner.ReCaptchaLoader = function() {
	var stack = [],
		stackContext = [],
		isLoaded = false;

	/**
	 * Load ReCaptcha API
	 */
	this.loadReCaptchaScript = function() {
		var lang = Runner.getGoogleLanguage(),
			langStr = lang? "&hl=" + lang : "";

		$('<script src="' + location.protocol + '//www.recaptcha.net/recaptcha/api.js?onload=reCaptchaLoaded' + langStr + '&render=explicit" async defer></script>').appendTo(document.getElementsByTagName("head")[0]);
	}

	this.onLoad = function( f, context ) {
		stack.push( f );
		stackContext.push( context );
		if ( this.isLoaded ) {
			this.fireLoad();
		}
	}

	this.fireLoad = function() {
		for (var i = 0; i < stack.length; i++) {
			stack[i].apply( stackContext[i] || this );
		}
		stack = [];
		stackContext = [];
	}

	this.reCaptchaLoaded = function() {
		this.isLoaded = true;
		this.fireLoad();
	}
};

var reCaptchaLoaded = function() {
	Runner.globalReCaptchaLoader.reCaptchaLoaded();
};

/**
 * Load ReCaptcha API
 */
(function() {
	Runner.globalReCaptchaLoader = new Runner.ReCaptchaLoader();
	Runner.globalReCaptchaLoader.loadReCaptchaScript();
}());