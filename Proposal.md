Table Of Contents
============
0. [Overview](#overview)
1. [REST](#rest)
	0. [Documentation](#documentation) 
	1. [Methods](#methods)
		1. [Proposed Methods and Definitions](#proposed-methods-and-definitions)
			1. [GET](#get)
				1. [Default Action](#default-action)
					1. [TODO - default action](#todo-default-action)
				2. [Paging](#paging)
				3. [Queries](#queries)
			2. [POST](#post)
			3. [PUT](#put)
			4. [DELETE](#delete)
			5. [PATCH](#patch)
				1. [TODO - op semantics](#todo-op-semantics)
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
			5. [409 Conflict](#409-conflict) 
			6. [422 Unprocessable Entity](#422-unprocessable-entity) 
			7. [500 Internal Server Error](#500-internal-server-error) 
	3. [Routing Functions/Actions](#routing-functionsactions)
	4. [Hypermedia as the Engine of Application State Recommendation](#hypermedia-as-the-engine-of-application-state-recommendation)
2. [Authentication](#authentication)
3. [Versioning](#versioning)
4. [Caching](#caching)

Overview
============
The proposal as outlined below is based largely on the previously assembled [API Documentation][api-documentation], which can be referred to for more in-depth analysis of the information and concepts presented below. 

REST
============

We should create a RESTful API, as much as reasonably possible. We may have a few exceptions as mentioned in [Routing Functions/Actions](#routing-functionsactions), but will strive for a coherent and cohesive API.

Documentation
------------

We should create fairly thorough documentation for each resource and HTTP Verb combination used. We should include the potential success status code(s), potential error status code(s), and examples of request and response bodies. As a group, we briefly discussed potentially using [RAML][raml], but not of us have done thorough research on it yet. I think there's a potential that using RAML would allow us to generate both the API Client and documentation URLs automatically - see [RAML Tools for .NET][raml-dotnet-tools].

Methods
------------

 Proposed Methods and Definitions
------------
The most commonly used HTTP Methods for a RESTful API are generally **POST**, **GET**, **PUT**, and **DELETE**, which roughly correspond to **Create**, **Read**, **Update**, **Delete** (**CRUD**). For some resources, we may also support **PATCH**.


#####**GET**#####
This corresponds to **Read** in **CRUD**. As listed in the expected characteristics above, we should treat **GETs** such that these requests should only perform read-only actions, and return a cacheable, deterministic response.

######**Default Action**######
By default, **GET** should return all the valid resources (e.g. /users GET will return all users).

######**TODO Default Action**######
Our general thoughts were that for "smart" clients, knowledgeable about the api system and not bad actors, returning all the things is reasonable and the "correct" result RESTfully speaking.

However, if we're thinking the API won't just be consumed internally, we probably want to limit the default response to something like the first page of results.

######**Paging**######
We should almost always support paging on **GET** Methods. Currently, we think something like:
```
GET /Users?page=1&itemsPerPage=50
```

Where the lower bound of page is 1. For handling out of upper bounds of page/itemsPerPage (e.g. if only 5 pages exist and a request is for page 7), redirect to the last available page. This prevents a potential class of annoying errors that would force bounds checking onto the client. Generally, take the view that we shouldn't throw clients under the bus for failing at bounds checking (especially since bounds can change between requests).

For invalid pages/itemsPerPage (e.g. values < 1, non-numeric, etc.), return an error (probably **422**).

######**Queries**######
We should, where appropriate, have the ability to limit the results of a **GET** with a query (in conjunction with paging), which will be handled as appropriate by the web api (e.g. via elasticsearch, database full text search, like query, etc.).


#####**POST**#####
This roughly corresponds to **Create** in **CRUD** when the ID/url of the given resource will be determined server-side. This should **NOT** be used for updates.

#####**PUT**#####
This usually corresponds to **Update** in **CRUD**, and occasionally **Create** as well. This should be used for **Create** **_ONLY_** when the client knows what the ID/url of the given resource will be. This is because semantically, we are **PUT**-ing ths requested resource at the URI called. **PUT** should probably only be called with a complete resource, as according to [RFC7231][rfc7231-4.3.4], a successful **PUT** suggests that a subsequent **GET** wull return the equivalent representation of the resource.

#####**DELETE**#####
This corresponds to **Delete** in **CRUD**. According to [RFC7231][rfc7231-4.3.5], this should return 202 if the server expects to delete the resource at a later time, 204 if the delete has been processed and no other information is to be shown, or 200 if the delete has been procesed and there's a response message of the new representation.

#####**PATCH**#####
This is used as for partial updates to resources. Our current proposal is to use a method like below, which, while slightly verbose, more easily supports complex operations such as copying and moving resources. 
```javascript
PATCH /users/123
[
	{ "op": "replace", "path": "/email", "value": "new.email@example.org" }
]
```
In general, we're going to try to follow [RFC6902][rfc6902] for patching, with the caveat that we only generally support one level down of patching a resource (e.g. **/Users/123/Email** and not **/Users/123/Addresses/1/Zipcode**) - any further down should be handled somewhere else if reasonable.
See [William Durand's post for more details][william-durand].

######**TODO op semantics**######
As per the [above link][william-durand] and [RFC6902 Section 4][rfc6902-4],
there are a limited set of "valid" operations. If we wanted, we could probably just allow our own operations (such as "reindex"), which would obviate issues of [Routing Functions/Actions](#routing-functionsactions).

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

Our error messages will be returned in the following form:
```javascript
{
    "errors": [{
        "message": "Limited to 50 characters",
        "property": "FirstName"
    }]
}
```
Where property is an empty string or null when the error is not tied to a particular property.
(**_Note_**: There was some discussion of using error codes like [Twitter's API][twitter-response-codes]. We're holding off on that for now, but since it's an additional field, it shouldn't be a breaking change if we add it in the future.)

In general, we should return errors such that we:

1. return both an appropriate HTTP status code and response body.
2. allow for an array of errors (that way we don't break compatibility if we initially thought just 1 error is ok, but then want to return multiple instead)	
3. for each error, return some appropriate text defining the error.
4. tie an error to a specific property of the request as appropriate for easier error handling client-side.

###Proposed Failure Status Codes###
#####**400 Bad Request**#####
The generic/default **_client_**-side error response. Technically, this means the request was syntactically invalid (although by convention it's used as a generic request error).

#####**401 Unauthorized**#####
The client is not authentication. (yes, this is a poorly named standard status code - based on [this stackoverflow question][so-403v401] most use it as **unauthenticated**, and instead uses 403 as **unauthorized**. Sorry. You can probably convince me that this is the correct code for both, though). As per [RFC7235][rfc7235-4.1], we MUST include the WWW-Authenticate header ([with OAuth 2, the challenge should be 'Bearer'][rfc6750-3]).

#####**403 Forbidden**#####
The client _is_ authenticated, but **_not_** authorized to perform the request.

#####**404 Not Found**#####
The request resource does not exist, or the server refuses to fulfill the request (e.g. 401 or 403), but for security purposes doesn't want to indicate that the resource exists.

#####**409 Conflict**#####
This is starting to be used in modern APIs to indicate a resource conflict if the request was fulfilled, such as trying to PUT a new resource where one already exists, or a DELETE that's not allowed due to cascading dependencies. This is occasionally used in place of 422 (see below), largely due to it being part of the HTTP 1.1 spec.
 
#####**422 Unprocessable Entity**#####
The request was syntactically formatted properly, but the data in the request is invalid. It's not technically part of the HTTP 1.1 spec, but rather [WebDAV][rfc4918-11.2], although it's becoming the preferred response code, as seen [here][so-400v422], [here][so-400v422-2], and [here][so-400v409v422].

#####**500 Internal Server Error**#####
The generic/default **_server_**-side error response.

Routing Functions/Actions
------------
For 'functional' actions that are difficult to describe in a RESTful way (e.g. what should the uri for 'rebuild lucene index' be?), probably the best approach for instances like this would be to [consider the state we want the resource to be in][rest-route-states]. This way, we're consistently using a RESTful paradigm instead of mix and match with functional RPC. So, we might have something like:
```javascript
/api/v1/Indexes/Users PATCH
[ { "op": "replace", "path": "/LastRebuildTime", "value": "{{DateTime.Now}}"} ]
```

On the other hand, it may make sense to just not try to force a square peg into a round hole, and simply have a verb-y route like:

	/api/v1/Indexes/Users/Rebuild POST

The consensus is to try as much as possible to adhere to a RESTful style of routing, likely using **PATCH** for such operations. If it's just too difficult or unwieldy to put into a RESTful route, we'll allow for exceptions. 


Hypermedia as the Engine of Application State Recommendation
------------

The consensus is that while HATEOAS sounds very useful, without some well-established protocols or standards and appropriate tooling, and with the primary API consumers being ourselves (thus making it easier to keep us all on a recent version of a web API client), it's likely too much effort to implement, for not enough payoff.

Authentication
============

The easiest and most reasonable authentication is probably a simple implementation of [OAuth 2.0 Bearer Token Usage][rfc6750].
This is used by roughly everyone with an API build in the last few years (e.g. twitter, Facebook, google, Microsoft, etc. etc.).

***Important Note***
SSL/TLS **MUST** be used, as anyone snooping that reads the bearer token can then use it as they please (until it expires, if ever).

The general consensus is is to use a simple OAuth token for both user and app authorization for non-public web API methods.

Versioning
============

The consensus is to version in the URL for the entire API (e.g. https://sole.hsc.wvu.edu/api/v2/users/ instead of https://sole.hsc.wvu.edu/api/users/v2/), and to keep one or two versions back in operation.

We should only increment versions when the response or request format changes, and we could possibly/probably get away with changing it only if new fields are required in the request, or the response removes old fields. That is, if a request made that's expecting it's working against the "previous" version is still valid for the "new" version, we don't need a new version number (e.g., the request now optionally takes a new field, but it's not required).


Caching
============

We've decided for now that caching likely isn't necessary for internal use only, for the following reasons:

1. The applications talking via the Web API are either located on the same physical servers, or within a few feet of each other, so response time likely isn't a large difference.
2. AppScan hates caching of data.

If either of these assumptions are wrong or change, caching likely wouldn't be too hard to implement - we'd probably just need one actionfilter (or other form of dataannotation) that hashing and compares the response body to the request etag, likely only on **GET** actions. See [API Documentation Caching][api-documentation-caching] for more information.

[api-documentation]: https://github.com/psantiago/ApiDocumentation
[api-documentation-caching]: https://github.com/psantiago/ApiDocumentation#caching
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
[raml]: http://raml.org/
[raml-dotnet-tools]: https://github.com/mulesoft-labs/raml-dotnet-tools
[readthedocs-restful-api]: http://restful-api-design.readthedocs.org/en/latest/intro.html
[rest-and-s3]: http://awsmedia.s3.amazonaws.com/pdf/RESTandS3.pdf
[rest-route-states]:  http://programmers.stackexchange.com/a/261647/172576
[restapitutorial-codes]: http://www.restapitutorial.com/httpstatuscodes.html
[restful-cookbook]: http://restcookbook.com/
[rfc4918-11.2]: https://tools.ietf.org/html/rfc4918#section-11.2
[rfc6750-3]: https://tools.ietf.org/html/rfc6750#section-3
[rfc6750]: http://tools.ietf.org/html/rfc6750
[rfc6902]: http://tools.ietf.org/html/rfc6902
[rfc6902-4]: http://tools.ietf.org/html/rfc6902#section-4
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
