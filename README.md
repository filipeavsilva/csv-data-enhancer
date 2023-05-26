# Transaction data enhancer

## Description
This project was created in the context of the Cardano coding assigment.
It provides an API which allows to upload CSV transaction data, which it then enriches with entity data from the [GLEIF API](https://documenter.getpostman.com/view/7679680/SVYrrxuU?version=latest).

A plan for deploying this solution can be found in the [Deployment Plan](deployment-plan.md).

## Functionality
This API provides the following endpoints:
* `GET /`: A GET request to the root works as a health check, to confirm that the API is running successfully
* `POST /enrich`: This endpoint expects CSV transaction data in the body. This data can be either as a raw body, with a Content-Type of `text/csv`, or as a file upload via an HTML form, with Content-Type `multipart/form-data`. Either way, the data is enriched following the logic required by the assignment, and returned in CSV format. This endpoint is meant to be used directly by users that require this functionality as part of e.g. a data processing script. See [the Using the API section](#using-the-api) below for ways to call this endpoint.
* `GET /enrich`: This endpoint exists for ease of access of users who don't want to create POST requests directly, or simply prefer using a web form. Meant to be opened in a browser, this endpoint renders a web page with a file upload form. That form allows to upload a CSV file, which then is processed by the endpoint above. The enriched data will then be interpreted by the browser as a file download.

## Running (locally)
This project can be run in a few different ways. Once running, the API will be available on port `7000` (if using HTTP) or `7100` (if using HTTPS).

This API was developed with .NET 7, and requires that version of the SDK to be installed to run.

* **IDE** (requires Visual Studio or a compatible IDE): Simply open the solution in Visual Studio or a similar IDE and run the API project, which should be the default.
* `dotnet run` (requires the `dotnet` command-line tool, included in the [.NET SDK](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net70)): From the root folder of the project, you can run the command `dotnet run --project API/API.csproj`
* **Docker** (requires Docker Engine installed): This project contains a `Dockerfile` that enables containerizing the API into a docker image, which can then be run in a container locally. To help simplify this process, the [scripts](scripts) folder contains two scripts, `run.sh` (for unix systems or others using an sh-compatible shell) and `run.ps1` (for Windows systems using Powershell). These scripts build the Docker image and run it in a container immediately. **NOTE:** Setting up the Docker image and the solution to allow HTTPS access to the container is a slightly complex process with which I am not very experienced, especially in order to allow another user to run it without requiring changes. I chose not to do this, and therefore the Docker container can only be accessed via HTTP, not HTTPS. 

## Using the API
To call the API there are also a few options:
* Using the browser: As mentioned [above](#functionality), accessing the `/enrich` endpoint with a browser will show a web page with a file upload form. An input CSV file can be uploaded there, and the resulting enriched data will be returned as a file download.
* Using [Postman](https://www.postman.com/). A Postman collection is also included in the root of this project. This can be imported into the Postman tool and used to upload a file (using the `binary` option) as the response body.
* Using the command-line: Being a regular HTTP API, the `POST /enrich` endpoint can be used through a tool such as `curl` or `wget`. This is ideal to use as part of a script that requires the enriched data. To simplify this task, the [scripts](scripts) folder contains two scripts,  `enrich.sh` (for unix systems or others using an sh-compatible shell, requires `curl` to be installed) and `run.ps1` (for Windows systems using Powershell). These scripts accept the path to a csv file, and use it to call the enrichment API. The resulting csv data is returned to the standard output, and can be placed into a file, or used as input for subsequent script commands.

# Assignment development notes

## Assumptions and decisions

* The target users for this API are assumed to have reasonable technical knowledge, in order to call a web API from a script or at least not need guidance to upload a CSV file 
* The solution was created as a web API for ease of access by as many users as possible, without requiring complex distribution (as e.g. an executable application would)
* Input data are not expected to contain very large amounts of lines. Processing input data is quite performant and should handle a few tens of thousands of lines without much issue (e.g. around 200,000 lines of data can be processed in around 3 seconds). However, a limiting factor is the number of distinct LEIs present in the data:
    * The GLEIF API imposes a rate limit of 60 requests per minute.
    * To minimize the risk of hitting this limit, the API requests entity information in batches, using the [filtering](https://documenter.getpostman.com/view/7679680/SVYrrxuU?version=latest#use-cases) functionality of the `/lei-records` endpoint. The batches were chosen to contain a maximum of 59 LEIs, to maximize the amount of data retrieved in a single request without hitting a limit on URL size.
    * The LEIs in the input data are gathered to ensure there are no repeated LEIs being requested to the GLEIF API. This also minimizes the number of requests necessary.
    * The maximum expected number of distinct LEIs accepted in the input data is 3540 (59 LEIs * 60 requests). Testing has shown the real limit to be higher, but this is the maximum level allowed by the documentation.
* The API was not designed to handle a heavy load of many users calling it simultaneously. Again, the limiting factor here is the rate limit on the GLEIF API. Multiple users with large files (with many distinct LEIs) could mean the API would make a large amount of requests from GLEIF very quickly.
* The API keeps all the data in memory at some points. If very large files (read: gigabyte-sized) are used this can quickly exhaust the available memory.
* If errors occur in the data (e.g. a LEI does not match a record, or a country is found that was not taken into account in the calculations) the result is that the specific line with the error will not be enriched, but will still be included in the output data. This is intended to generate the least amount of impedance, while still allowing for easy checking of any issues. Ideally this would be discussed further with the users in an agile manner, and iterated on quickly.
* Unit testing is in place for some classes but having a working solution took priority
* Security was not a big concern while working on this assignment, other than ensure HTTPS communication. In a production scenario access control would be necessary.

## Improvement points
In this section I would like to debate possible improvement points if this would be implemented as a production system rather than as an assignment, i.e. considerations I would like to take into account with more time and for a production system:

* Logging and monitoring. I did not add logging or monitoring in this assignment, but (as mentioned in the Deployment plan) these would be essential to understand the usage patterns of the users of the API and be on top of any issues. See the [Deployment plan](deployment-plan.md) for my thoughts on this.
* Testing. While priorities and limited time led me to not add much test coverage, I would like to implement unit- and integration tests for the whole solution to guarantee correctness and minimize the chance of failure.
* Caching. The legal entity data doesn't change very often, being updated three times per day. This makes it a good candidate for caching, reducing the need to call the GLEIF API and improving performance.
* Stream input and output data instead of keeping it in memory. A possible performance improvement would be to stream both the input data, CSV parsing, and output. This would, however, impact the capacity of batching the LEIs on requests from the GLEIF API and some tradeoff would need to be found.
* Improve multiple user support. As mentioned above, a limiting factor here is the rate limit on the GLEIF API. That is made worse by the fact that this API is not aware of requests made by different users, and in the case of heavy usage by multiple users we could run into the rate limit. One possible improvement would be to distribute the requests, using a centralized system (which could even be a separate API) to gather requests for all users, batch them by LEI and return the enriched data to the appropriate users. This would mean that, no matter how many users would access the API simultaneously, the number of distinct LEIs _across all requests_ would now be the only limiting factor.