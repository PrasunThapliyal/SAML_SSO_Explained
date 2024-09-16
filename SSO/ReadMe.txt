16 Sept 2024
============

Goal
	Understand How SAML SSO works using an external IdP such as Okta

There are 2 Visual Studio Solutions
1. SP - Dotnet core WebAPI for the Service Provider, including UI served as static files
	This is running at https://localhost:7012
2. IdP - Dotnet core WebAPI for the Identity Provider, including UI served as static files
	This is running at https://localhost:7098

Run both IdP and SP
In the SP UI, click 'Login'.
The UI navigates from :7012 to :7098 (i.e from SP website to IdP website)
A login form is shown, enter admin/password and click 'Login'
The UI navigates from :7098 to :7012 (i.e from IdP website to SP website)

--------------------------------------------------------------------------
Learning: 
	Redirects using HTTP 302

	If the Javascript handler of 'login' button does a 'fetch' to its api /auth/login and receives a 302 with Location: https://Idp/login
	Upon receiving the 302 status response, the browser JS engine will fire another GET request to remote endpoint (Location header)
	and you will get the response HTML displayed in the dev tools window. But the browser still remains on the SP website, 
	i.e. the URL is not changed in the address bar

	However, type the same https://SP/auth/login on the address bar and hit enter. This time the 302 is received by the brwoser rather than its JS engnig,
	and the browser (just like the JS engine) is well behaved and calls IdP, but it does via its url change
	i.e. This results in a page navigation, which is what we want in this scenario

	So there are 2 ways to get the desired result:
	1. The 'login' handler does a window.location.href = '/auth/login';
		The change in URL leads to a GET call in the SP's Auth Controller. So the 302 sent by auth controller is received by the browser
		And page then navigates to IdP (follow Location header)
	2. The second way is to call /auth/login via fetch call, and handle the response in JS, 
		and then do window.location.href = 'https://IdP/auth/login';
	We are following the first approach - seems cleaner
--------------------------------------------------------------------------
Flow:
	User opens https://SP/index.html and clicks the 'Login' button
	login handler does a window.location.href = '/auth/login';
		This calls SP's auth controller.
		Auth Controller constructs SAML Request and Relay State (and maybe a CSRF token, not done in this example)
		Returns 302 with Location Header = https://IdP/auth/login?samlRequest=blah&relayState=blah
	Browser navigates to IdP website and fires a GET to https://IdP/auth/login endpoint

	On the IdP, the call is received by Auth Controller's /auth/login endpoint
	Parse the SAML request - do required security checks, and then 
	return 302 to /login.html?SAMLRequest={Uri.EscapeDataString(decodedSAMLRequest)}&RelayState={Uri.EscapeDataString(RelayState)}

	The browser again redirects - this time within IdP's website to its login page
	The Login Page's on load event extracts the SAML request and Relay state from the url parameters 
	- note that so far we are just bouncing the SAML request (and relay state) around so that it is available throught the flow

	The login page has username and password field. User enters and clicks Submit
	Submit handler does a fetch to /auth/validate, where the IdP backend validates credentials
	If the credentials are valid, it constructs a SAML response and returns it as a 200 Ok (along with Relay State)

	When the JS handler receives a 200 ok, it constructs a new 'Form'
	- this is an auto submitting form with form.action = https://SP/auth/saml/acs and form.method='POST'
	- the form has fields such as SAMLResponse and RelayState
	- these fields are of type 'hidden'
	This form is 'appended' to the current page by document.body.appendChild(form);
		Since the fields are hidden, nothing gets visible on the IdP UI which is still at the Login page

	Since its an auto submitting form the submit happens as soon as /auth/validate returns 200 Ok,
	and as a result of submit, the brwoser does a POST to https://SP/auth/saml/acs

	The acs endpoint of SP also does its security thing with the SAML response, 
	and at the end redirects to its own /designs.html (or whatever is in its relayState)
	The 302 also contains a SetHeader which installs a user session cookie on domain https://SP/

	Browser is now at SP website and user is logged in, cookied installed
--------------------------------------------------------------------------
	The IdP would also install a cookie at https://IdP
	We haven't done this in our example, but its on similar lines as SP

	So imagine we have SP2 and the same user wants to login, when the page is redirected to IdP, since IdP's cookie is already present
	IdP need not redirect the user to login page again, and simply return to SP2's acs endpoint
	So the user is not required to enter credentials again

	This is SSO
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------
--------------------------------------------------------------------------






