Table Of Contents
============
1. [REST](#rest)
	1. [Methods](#methods)
		1. [Expected Characteristics](#expected-characteristics)
			1. [Safe](#safe)
			2. [Idempotent](#idempotent)
			3. [Cacheable](#cacheable)
		2. [Proposed Methods and Definitions](#proposed-methods-and-definitions)
			1. [GET](#get)
			2. [POST](#post)
			3. [PUT](#put)
			4. [(Sidebar: POST vs PUT)](#post-vs-put)
			5. [DELETE](#delete)
			6. [PATCH (**_optional_**)](#patch)
	2. [HTTP Status Codes](#http-status-codes)
		1. [Successful Requests](#successful-requests)
		2. [Proposed Success Status Codes](#proposed-success-status-codes)
			1. [200 OK](#200-ok)
			2. [201 Created](#201-created)
			3. [204 No Content](#204-no-content)
			4. [304 Not Modified](#304-not-modified)
		3. [Failed Requests](#failed-requests)
		4. [Proposed Failure Status Codes](#proposed-failure-status-codes)
			1. [400 Bad Request](#400-bad-request) 
			2. [401 Unauthorized](#401-unauthorized) 
			3. [403 Forbidden](#403-forbidden) 
			4. [404 Not Found](#404-not-found) 
			5. [409 Conflict (**_optional_**)](#409-conflict-optional) 
			6. [422 Unprocessable Entity (**_optional_**)](#422-unprocessable-entity-optional) 
			7. [500 Internal Server Error](#500-internal-server-error) 
	3. [Routing Functions/Actions](#routing-functionsactions)
	4. [Hypermedia as the Engine of Application State (**_optional_**)](#hypermedia-as-the-engine-of-application-state-optional)
		1. [HATEOAS Example](#hateoas-example)
		2. [Recommendation](#hateoas-recommendation)
		3. [Additional HATEOAS/Hypermedia Reading](#additional-hateoashypermedia-reading)
2. [Authentication](#authentication)
3. [Versioning](#versioning)
4. [Caching](#caching)
5. [General API Guides & Other Resources](#general-api-guidesother-resources)

REST
============

We should create a RESTful API, because by implementing a RESTful architecture over HTTP, we gain the benefit of the HTTP Specification - i.e. a reasonably well-defined way to describe and interact with resources that we're already using as web developers. This also makes reasoning about caching, error handling, etc. a matter of considering the HTTP Specification, rather than defining our own architectural conventions as we would have to with messaging protocols such as SOAP, JSON-RPC, or something more ad-hoc (though those could also be implemented with a restful architecture). There are probably other good reasons that someone smarter than me can enumerate here. Otherwise, the rest of my argument ends up being ad populum and ad verecundiam.


Methods
------------

###Expected Characteristics
Generally speaking, I think as long as we're consistent with the verbs we use, we should be fine. That said, we should probably keep in mind the defined characteristics of various HTTP Verbs as defined by [RFC 7231][rfc7231-4.2] - these should largely inform when and why we should use certain verbs, and what we should do on the server-side when processing these.
   
#####**Safe**#####
>Request methods are considered "safe" if their defined semantics are   essentially read-only; i.e., the client does not request, and does not expect, any state change on the origin server as a result of applying a safe method to a target resource.  Likewise, reasonable use of a safe method is not expected to cause any harm, loss of property, or unusual burden on the origin server.
   
#####**Idempotent**#####
>A request method is considered "idempotent" if the intended effect on the server of multiple identical requests with that method is the same as the effect for a single such request.
   
#####**Cacheable**#####
>Request methods can be defined as "cacheable" to indicate that responses to them are allowed to be stored for future reuse....

| Verb   | Safe | Idempotent | Cacheable
| ------ | ---- | ---------- | ---------
| GET    | Yes  | Yes        | Yes
| POST   | No   | No         | Only if the response explicitly returns the resource
| PUT    | No   | Yes        | No
| DELETE | No   | Yes        | No
| PATCH  | No   | No         | Only if the response explicitly returns the resource

 Proposed Methods and Definitions
------------
The most commonly used HTTP Methods for a RESTful API are generally **POST**, **GET**, **PUT**, and **DELETE**, which roughly correspond to **Create**, **Read**, **Update**, **Delete** (**CRUD**).


#####**GET**#####
This corresponds to **Read** in **CRUD**. As listed in the expected characteristics above, we should treat **GETs** such that these requests should only perform read-only actions, and return a cacheable, deterministic response.

#####**POST**#####
This roughly corresponds to **Create** in **CRUD** when the ID/url of the given resource will be determined server-side. This should **NOT** be used for updates.

#####**PUT**#####
This usually corresponds to **Update** in **CRUD**, and occasionally **Create** as well. This should be used for **Create** **_ONLY_** when the client knows what the ID/url of the given resource will be. This is because semantically, we are **PUT**-ing ths requested resource at the URI called. **PUT** should probably only be called with a complete resource, as according to [RFC7231][rfc7231-4.3.4], a successful **PUT** suggests that a subsequent **GET** wull return the equivalent representation of the resource.


>#####**POST vs PUT**#####
>For those so inclined, the precise usages of **POST** vs **PUT** are quibble-able as seen [here][roy-okay-post], [there][so-put-v-post], [and][cookbook-put-v-post] [everywhere][rest-and-s3], generally, at least more recently, along the lines that if the client can determine the uri based on the resource (e.g. with a non-auto-incremented id), you should use **PUT** for **Create** **_and_** **Update** (as noted in the above recommendations). That's probably more "correct" in terms of RFC7231, although like most considerations, I don't think it matters really, as long as we're consistent.

#####**DELETE**#####
This corresponds to **Delete** in **CRUD**. According to [RFC7231][rfc7231-4.3.5], this should return 202 if the server expects to delete the resource at a later time, 204 if the delete has been processed and no other information is to be shown, or 200 if the delete has been procesed and there's a response message of the new representation.

#####**PATCH**#####
This is occasionally used as well for partial updates to resources. Some people will do the simplest option:
```javascript
PATCH /users/123
{ "email": "new.email@example.org" }
```

[While others insist that's entirely nonstandard and everyone should know the only correct patch bodies are written like][william-durand]:
```javascript
PATCH /users/123
[
	{ "op": "replace", "path": "/email", "value": "new.email@example.org" }
]
```

HTTP Status Codes
------------
We should use a consistent set of HTTP Status Codes. Below are some recommended, generally used status codes returned by RESTful APIs. See:

- [REST API Tutorial][restapitutorial-codes]
- [Web API Design - Crafting Interfaces that Developers Love][apigee-api-guide]
- [Twitter API response codes][twitter-response-codes] 

I've also marked some status codes as (**_Optional_**), for less frequently used, more granular status codes that can be represented with a different code.

###Successful Requests###
Successful requests should return 200 or 300 level status codes, and whatever responses are deemed appropriate.

###Proposed Success Status Codes###
#####**200 OK**#####
The generic/default success response, when we don't have a more specific success response that's preferable.

#####**201 Created**#####
The newly requested resource was created without errors.

#####**204 No Content**#####
Generally used in REST APIs when deleting a resource is successful.

#####**304 Not Modified**#####
The client's cached response should be used.

###Failed Requests###

Failed requests should return 400 level responses if the error is due the request as specified by the client, or a 500 level response if the error is due to server-side processing of the request.

Along with the appropriate HTTP Status Code, we should also return a standardized error response body. 

[Twitter's API][twitter-response-codes] returns errors bodies in the following form;
```javascript
{
    "errors": [{
        "message": "Sorry, that page does not exist",
        "code": 34
    }]
}
```

[Apigee][apigee-api-guide] and [m-way][mway-api-guide] both recommend slightly more verbose response bodies, roughly in the form:
```javascript
{
    "errors": [{
        "userMessage": "Sorry, the requested resource does not exist",
        "internalMessage": "No car found in the database",
        "code": 34,
        "more info": "http://dev.mwaysolutions.com/blog/api/v1/errors/12345"
    }]
}
```

Either way, we should return errors such that we:

1. return both an appropriate status code and response body.
2. allow for an array of errors (that way we don't break compatibility if we initially thought just 1 error is ok, but then want to return multiple instead)	
3. for each error, return some appropriate text defining the error

###Proposed Failure Status Codes###
#####**400 Bad Request**#####
The generic/default **_client_**-side error response. Technically, this means the request was syntactically invalid (although by convention it's used as a generic request error).

#####**401 Unauthorized**#####
The client is not authentication. (yes, this is a poorly named standard status code - based on [this stackoverflow question][so-403v401] most use it as **unauthenticated**, and instead uses 403 as **unauthorized**. Sorry. You can probably convince me that this is the correct code for both, though). As per [RFC7235][rfc7235-4.1], we MUST include the WWW-Authenticate header ([with OAuth 2, the challenge should be 'Bearer'][rfc6750-3]).

#####**403 Forbidden**#####
The client _is_ authenticated, but **_not_** authorized to perform the request.

#####**404 Not Found**#####
The request resource does not exist, or the server refuses to fulfill the request (e.g. 401 or 403), but for security purposes doesn't want to indicate that the resource exists.

#####**409 Conflict** (**_Optional_**)#####
This is starting to be used in modern APIs to indicate a resource conflict if the request was fulfilled, such as trying to PUT a new resource where one already exists, or a DELETE that's not allowed due to cascading dependencies. This is occasionally used in place of 422 (see below), largely due to it being part of the HTTP 1.1 spec.
 
#####**422 Unprocessable Entity** (**_Optional_**)#####
The request was syntactically formatted properly, but the data in the request is invalid. It's not technically part of the HTTP 1.1 spec, but rather [WebDAV][rfc4918-11.2], although it's becoming the preferred response code, as seen [here][so-400v422], [here][so-400v422-2], and [here][so-400v409v422].

#####**500 Internal Server Error**#####
The generic/default **_server_**-side error response.

Routing Functions/Actions
------------
For 'functional' actions that are difficult to describe in a RESTful way (e.g. what should the uri for 'rebuild lucene index' be?), probably the best approach for instances like this would be to [consider the state we want the resource to be in][rest-route-states]. This way, we're consistently using a RESTful paradigm instead of mix and match with functional RPC. So, we might have something like:

    /api/v1/Indexes/Users PATCH
    { "LastRebuildTime": "{{DateTime.Now}}" }

On the other hand, it may make sense to just not try to force a square peg into a round hole, and simply have a verb-y route like:

	/api/v1/Indexes/Users/Rebuild POST

I think the first option makes the most sense, but I don't really have too strong of an opinion on this either, we should just be consistent throughout the API.


Hypermedia as the Engine of Application State (**_optional_**)
------------

According to [Roy Fielding][roy-fielding], one of the pricipal authors of the HTTP Specification, and whose dissertation first described REST:
>...if the engine of application state (and hence the API) is not being driven by hypertext, then it cannot be RESTful and cannot be a REST API. Period. Is there some broken manual somewhere that needs to be fixed?
>
>[from [REST APIs must be hypertext-driven][roy-hypertext-driven]]

Hypermedia as the Engine of Application State, horrendously acronym'd as **HATEOAS**, is the idea that the response from a REST API should indicate all the actions/state transitions the client can perform on that response, much in the same way that a rendered HTML page indicates to a user what actions/ state transitions they can perform via links/buttons/etc.

The biggest benefit to HATEOAS, as far as I can tell, is the ability to keep logic on the server-side, rather than needing to be reimplemented client-side. See the example below.

###HATEOAS Example###
The usual example is given and stolen from [here][what-is-hateoas].
For a bank account in good standing, you might get a response like:

```xml
GET /account/12345 HTTP/1.1 HTTP/1.1 200 OK 

<?xml version="1.0"?> 
<account> 
	<account_number>12345</account_number> 
	<balance currency="usd">100.00</balance>
	<link rel="deposit" href="/account/12345/deposit" />
	<link rel="withdraw" href="/account/12345/withdraw" />
	<link rel="transfer" href="/account/12345/transfer" />
	<link rel="close" href="/account/12345/close" />
</account>
```

Whereas an overdrawn account might have a response like:

```xml
GET /account/12345 HTTP/1.1 HTTP/1.1 200 OK 
<?xml version="1.0"?> 
	<account>
	<account_number>12345</account_number>
	<balance currency="usd">-25.00</balance>
	<link rel="deposit" href="/account/12345/deposit" />
</account>
```
The point being that the business logic of determining what actions can be performed on a resource is _behind_ the API, rather than needing to be duplicated by the API client. In this example, the API client doesn't need to have any sort of knowledge about what a negative balance means in relation to what actions can be taken - this is all determined by the api response. This makes the client and API server less coupled, as the server is free to change the logic.

###HATEOAS Recommendation###

I have pretty mixed feelings on whether or not it's worthwhile to implement HATEOAS. I think at it's simplest, links-only level, it _may_ be worth the effort, but probably not at the most complex, and arguable most correct way that would allow for an entirely self-discoverable client.

On the one hand, the benefit of using a simple(-ish) format like [HAL][hal] or [JSON API][json-api] that basically just adds links for actions/state transitions (similar to the example above) seems to provide the benefit of looser coupling to the server. The downside is that it's of course more work to keep track of valid state transitions, we still potentially need client-side business logic anyway for simple validation, and the simplest formats I could find, HAL and JSON API, don't allow for a completely self-discoverable API (e.g. it has no notion of a way to provide the schema of what we're putting/posting/etc. on links, just that the links exist). 

There are other specifications like [JSON Schema with Hyper-schema (example)][json-schema-example], [JSON-LD and HYDRA][json-ld-hydra], and [Siren][siren], that look like they can fully describe a REST API, including HTTP Methods and the schemas of those requests and responses, but this is probably a **_huge_** amount of effort and overkill for us, especially since a lot of the tooling isn't really there yet for C#. 

###Additional HATEOAS/Hypermedia Reading###

- API Best Practices: Hypermedia [Part 1][mulesoft-hypermedia-1], [Part 2][mulesoft-hypermedia-2], [Part 3][mulesoft-hypermedia-3]
- [Choosing a Hypermedia Format][choosing-hypermedia-format]
- [Collection+JSON (yet another hypermedia format)][collection-json]
- [Elegant APIs with JSON Schema][json-schema-example]
- Harnessing HATEOAS [Part 1][harnessing-hateoas-1] and [Part 2][harnessing-hateoas-2]
- [Honing in on HATEOAS][honing-on-hateoas]
- [What's the point with HATEOAS on the client-side?][so-whats-point-of-hateoas]

Authentication
============

The easiest and most reasonable authentication is probably a simple implementation of [OAuth 2.0 Bearer Token Usage][rfc6750].
This is used by roughly everyone with an API build in the last few years (e.g. twitter, Facebook, google, Microsoft, etc. etc.).

***Important Note***
SSL/TLS **MUST** be used, as anyone snooping that reads the bearer token can then use it as they please (until it expires, if ever).

If we want to use an authentication scheme that doesn't rely on SSL for security, we could instead use something like OAuth 1.0 (which uses a private/public key signature scheme) or the possibly defunct [HTTP MAC Authentication Scheme][mac-auth] or equally possibly defunct [OAuth 2.0 Message Authentication Code (MAC) Tokens][mac-token]).

An additional consideration would be building upon a simple bearer token with [JSON Web Tokens][rfc7519]. The benefit over simple bearer tokens is the inclusion of whatever information you want (generally some basic user data, and what they have access to), which has a server-side only validable signature (so someone can't reasonably fake a valid token without the secret keys). The idea is that by including user authorization information in every request, we prevent needing to do database lookups to determine user access (and in our case, it could also prevent api calls to sole, since we would have the user access able to be determined by inspecting the token itself). The downside is that with a large enough amount of data (since it's just base64 encoded, and headers in http 1.1 aren't compressible), you can end up requiring a large authorization header in every request, which defeats the purpose.

Versioning
============

Versioning is important. 
We should have it in some way, and probably require it for api calls.
We should try really hard to treat the API as an interface that we won't be changing frequently (after the initial release).

We should only increment versions when the response or request format changes, and we could possibly/probably get away with changing it only if new fields are required in the request, or the response removes old fields. That is, if a request made that's expecting it's working against the "previous" version is still valid for the "new" version, we don't need a new version number (e.g., the request now optionally takes a new field, but it's not required).

Generally, _most_ people seem to recommend versioning the entire api, instead of each resource individually (e.g. /api/v1/cats instead of /api/cats/v1), mostly because it's easier to reason about, and easier to say "we still support x-2 versions".

The location of versioning can easily lead to [global thermonuclear war][so-versioning]. Here's a quick and reasonable explanation which I'll now steal wholesale from [Your API versioning is wrong, which is why I decided to do it 3 different wrong ways][troy-hunt-versioning] (which is a good resource):

>###The various versioning camps###
> Right, so how hard can this versioning business be? I mean it should be a simple exercise, right? The problem is that it gets very philosophical, but rather than get bogged down in that for now, let me outline the three common schools of thought in terms of how they’re practically implemented:

> 1. **URL**: You simply whack the API version into the URL, for example: https://haveibeenpwned.com/api/v2/breachedaccount/foo
> 2. **Custom request header**: You use the same URL as before but add a header such as “api-version: 2”
> 3. **Accept header**: You modify the accept header to specify the version, for example “Accept: application/vnd.haveibeenpwned.v2+json”

> There have been many, many things written on this and I’m going to link to them at the end of the post, but here’s the abridged version:

> 1. **URLs suck because they should represent the entity**: I actually kinda agree with this insofar as the entity I’m retrieving is a breached account, not a version of the breached account. Semantically, it’s not really correct but damn it’s easy to use!
> 2. **Custom request headers suck because it’s not really a semantic way of describing the resource**: The HTTP spec gives us a means of requesting the nature we’d like the resource represented in by way of the accept header, why reproduce this?
> 3. **Accept headers suck because they’re harder to test**: I can no longer just give someone a URL and say “Here, click this”, rather they have to carefully construct the request and configure the accept header appropriately.

> The various arguments for and against each approach tend to go from “This is the ‘right’ way to do it but is less practical” through to “This is the easiest way to create something consumable which therefore makes it ‘right’”. There is much discussion about hypermedia, content negotiation, what is “REST” and all manner of other issues. Unfortunately this very often gets philosophical and loses sight of what the real goal should be: building software that works and particularly for an API, making it easily consumable.

Personally, I think the Accept header makes the most sense is a RESTy and [Cool URIs don't change][such-cool] way, but it's mostly a weak opinion weakly-held.

Caching
============

We should consider caching all GET responses to improve performance. Probably the simplest and most reliable method would be to use [HTTP ETag][etag] (specified in [RFC7232][rfc7232]) with an MD5 hash of the response body. ETags still require a request to the server, but the response body may not be necessary if the etag of the resource hasn't changed compared to the requested etag. We can also guarantee (assuming we don't count hash collisions) that the most up-to-date version of the resource will always be retrieved from either the cache or in the response body. The alternative caching mechanism is to use Cache-Control/Expires, which does prevent a request, but may respond with stale data.

System.Net.WebClient does support caching, but from some quick testing, it appears that it must be specified:

```c#	
var client = new System.Net.WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default) };
```
Otherwise, it defaults to null, and no caching appears to happen.	

Additional Resources:

- [Increasing Application Performance with HTTP Cache Headers](heroku-caching)
- [Things Caches Do][things-caches-do]

General API Guides/Other Resources
============

- [The RESTful CookBook][restful-cookbook]
- [Do you really know why you prefer REST over RPC?][apihandyman-why-prefer-rest-over-rpc]
- [Best Practices for Designing a Pragmatic RESTful API][sahni-best-practices]
- [Designing your RESTful API for Longevity][mulesoft-designing-restful-api-longevity]
- [Web API Design - Crafting Interfaces that Developers Love][apigee-api-guide]
- [RESTful API Design][readthedocs-restful-api]
- [5 Golden Rules for Great Web API Design][toptal-5-golden-rules]


[apigee-api-guide]: https://pages.apigee.com/rs/apigee/images/api-design-ebook-2012-03.pdf
[apihandyman-why-prefer-rest-over-rpc]: http://apihandyman.io/do-you-really-know-why-you-prefer-rest-over-rpc/
[choosing-hypermedia-format]: http://sookocheff.com/posts/2014-03-11-on-choosing-a-hypermedia-format/
[collection-json]: http://amundsen.com/media-types/collection/
[cookbook-put-v-post]: http://restcookbook.com/HTTP%20Methods/put-vs-post/
[etag]: http://en.wikipedia.org/wiki/HTTP_ETag
[hal]: http://stateless.co/hal_specification.html
[harnessing-hateoas-1]: https://blog.apigee.com/detail/api_design_harnessing_hateoas_part_1
[harnessing-hateoas-2]: https://blog.apigee.com/detail/api_design_harnessing_hateoas_part_2
[heroku-caching]: https://devcenter.heroku.com/articles/increasing-application-performance-with-http-cache-headers
[honing-on-hateoas]: https://blog.apigee.com/detail/api_design_honing_in_on_hateoas
[json-api]: http://jsonapi.org/
[json-ld-hydra]: http://www.markus-lanthaler.com/hydra/
[json-schema-example]: https://brandur.org/elegant-apis
[mac-auth]: http://tools.ietf.org/html/draft-hammer-oauth-v2-mac-token-05
[mac-token]: http://tools.ietf.org/html/draft-ietf-oauth-v2-http-mac-05
[mulesoft-designing-restful-api-longevity]: http://blogs.mulesoft.org/designing-restful-api-longevity/
[mulesoft-hypermedia-1]: http://blogs.mulesoft.com/api-best-practices-hypermedia-part-1/
[mulesoft-hypermedia-2]: http://blogs.mulesoft.com/api-best-practices-hypermedia-part-2/
[mulesoft-hypermedia-3]: http://blogs.mulesoft.com/api-best-practices-hypermedia-part-3/
[mway-api-guide]: http://blog.mwaysolutions.com/2014/06/05/10-best-practices-for-better-restful-api/
[readthedocs-restful-api]: http://restful-api-design.readthedocs.org/en/latest/intro.html
[rest-and-s3]: http://awsmedia.s3.amazonaws.com/pdf/RESTandS3.pdf
[rest-route-states]:  http://programmers.stackexchange.com/a/261647/172576
[restapitutorial-codes]: http://www.restapitutorial.com/httpstatuscodes.html
[restful-cookbook]: http://restcookbook.com/
[rfc4918-11.2]: https://tools.ietf.org/html/rfc4918#section-11.2
[rfc6750-3]: https://tools.ietf.org/html/rfc6750#section-3
[rfc6750]: http://tools.ietf.org/html/rfc6750
[rfc7231-4.2]: http://tools.ietf.org/html/rfc7231#section-4.2
[rfc7231-4.3.4]: http://tools.ietf.org/html/rfc7231#section-4.3.4
[rfc7231-4.3.5]: http://tools.ietf.org/html/rfc7231#section-4.3.5
[rfc7232]: http://tools.ietf.org/html/rfc7232#section-2.3
[rfc7235-4.1]: http://tools.ietf.org/html/rfc7235#section-4.1
[rfc7519]: https://tools.ietf.org/html/rfc7519
[roy-fielding]: http://en.wikipedia.org/wiki/Roy_Fielding
[roy-hypertext-driven]: http://roy.gbiv.com/untangled/2008/rest-apis-must-be-hypertext-driven
[roy-okay-post]: http://roy.gbiv.com/untangled/2009/it-is-okay-to-use-post
[sahni-best-practices]: http://www.vinaysahni.com/best-practices-for-a-pragmatic-restful-api
[siren]: https://github.com/kevinswiber/siren
[so-400v409v422]: http://stackoverflow.com/a/2194786/957829
[so-400v422-2]: http://stackoverflow.com/questions/16133923/400-vs-422-response-to-post-of-data
[so-400v422]: http://stackoverflow.com/questions/22358281/400-vs-422-response-to-post-that-references-an-unknown-entity
[so-403v401]: http://stackoverflow.com/questions/3297048/403-forbidden-vs-401-unauthorized-http-responses
[so-put-v-post]: http://stackoverflow.com/questions/630453/put-vs-post-in-rest
[so-versioning]: http://stackoverflow.com/questions/389169/best-practices-for-api-versioning
[so-whats-point-of-hateoas]: http://programmers.stackexchange.com/questions/272532/whats-the-point-with-hateoas-on-the-client-side
[such-cool]: http://www.w3.org/Provider/Style/URI.html
[things-caches-do]: http://2ndscale.com/rtomayko/2008/things-caches-do
[toptal-5-golden-rules]: http://www.toptal.com/api-developers/5-golden-rules-for-designing-a-great-web-api
[troy-hunt-versioning]: http://www.troyhunt.com/2014/02/your-api-versioning-is-wrong-which-is.html
[twitter-response-codes]: https://dev.twitter.com/overview/api/response-codes
[what-is-hateoas]: http://restcookbook.com/Basics/hateoas/
[william-durand]: http://williamdurand.fr/2014/02/14/please-do-not-patch-like-an-idiot/
