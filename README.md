# Web Scraping

Deliver an API that returns the total number of lines and the total number of bytes of all the files of a given public Github repository, grouped by file extension.
Data is retrieved using pure web scraping techniques, without Github's API or web scraping libraries.

## Play

``GET`` https://web-scraping-paulera.herokuapp.com/api/WebScraping?url=&api_key=

- ``url``: Public GitHub repository, e.g. `https://github.com/paulojsilva/web-scraping`
- ``api_key``: If Authentication enabled, set the security key

## Tecnologies

Web application developed in ASP NET Core 3.1, C# 8.0 with:

- DDD (Domain Driven Design)
- Notification pattern
- Semaphore process synchronization
- Parallelism (ParallelForEach)
- MemoryCache (native)
- Redis Cache (StackExchange.Redis)
- HttpClient (native)
- AngleSharp (HTML parser)
- Unit tests with XUnit and FluentAssertions
- Docker

## Project

Project was modeled with DDD (Domain Driven Design) to guide the construction based on Domain.
The Domain has knowledge to how find informations with WebScraping on GitHub, deal with HTTP Requests, parallelism, etc.

**Layers**:

- **Api**: Expose on Web one API that has an URL request to aplicate WebScraping. It relates to the Application and delegates to it the responsibility of performing WebScraping operation.
- **Application**: Control Layer responsible to receive WebScraping requests and delegate to superior layer - Domain. The Application does not have knowledge on how to apply WebScraping.
- **Domain**: Main Layer that has knowledge on how to apply WebScraping.
- **Domain.Shared**: Concentrates shareable DTO - Data Transfer Objects across application and system configuration objects - `appsettings.json`
- **CrossCutting**: Transverse layer the others. Concentrates extensions and exceptions used in every project.
- **CrossCutting.IoC**: Limited to Dependency Injection. The IoC here (Inversion of Control) was used in the sense of just injecting the service into the customer, instead of the customer looking for and building the service he will use.
- **Infra.Data**:
	- Has elements relataded to Data. In our case, to Cache Service. 
	- But could establish repositories (Cache, NoSQL, Relational), or even concentrate logic to get HTML data (the mainstream of WebScraping)
	- HTML Data Repositories was not created here, because the complexity of working with recursively and in parallel is high, so to segregating HTML Data in another layer would further increase the complexity.

## Domain in Details

Upon receiving the request, the domain instantiates HttpClient - responsible for requesting HTML pages from GitHub and Semaphore - responsible for controlling parallel access to HttpClient. 

The `ProcessAsync` method is called for the 1st time to fetch the HTML from the 1st page.
The AngleSharp lib is used as HTML Parser (fantastic by the way!) And we ask questions like:

- Does the received HTML represent a GitHub directory listing page? 
- Or does it represent the contents (lines) of a file?

With that question answered, we were able to determine whether:

- Call `ProcessAsync` recursively to load another directory listing page
- Or if we look in the DOM for the number of lines and size (in bytes)

As the archive content pages are found, the files found (name, lines and byte size) are stored in the `ConcurrentBag temporaryFiles`.
Doing this process recursively and 1-by-1 is very slow. Then, we use ParallelForEach to give a UP on the recursion.

However, GitHub controls access to its resources with **Rate Limiting**, so if we consume many pages in a short time, GitHub blocks access with **429 Too Many Requests**.
To get around this, we use Semaphore, which limits the number of tasks that use a certain resource (in our case, it limits the number of tasks that access HttpClient).

## Heroku Deploy

- Create a [Hekoru account](https://www.heroku.com/)
- Create new app, like **web-scraping-paulera** =D
- Download and install [Heroku CLI](https://devcenter.heroku.com/articles/heroku-command-line)
- Configure ``Dockerfile`` (need to be in the same folder as .sln file)
- Navigate to Dockerfile folder and run:
- Docker build image: ``docker build --rm -f "Dockerfile" -t "web-scraping-paulera:latest" .``
- Login to Heroku (will open the browser): ``heroku login``
- Sign into Container Registry: ``heroku container:login``
- Push docker image: ``heroku container:push web -a web-scraping-paulera``
- Release the newly pushed images to deploy your app: ``heroku container:release web -a web-scraping-paulera``
- Its done! If you need check logs/errors/warnings, run: ``heroku logs --tail -a web-scraping-paulera``
- My deployed app is: https://web-scraping-paulera.herokuapp.com/

## Docker Hub

- Create a [Docker ID (account)](https://hub.docker.com/)
- Create a repository
- Navigate to Dockerfile folder and run:
- build image: ``docker build --rm -f "Dockerfile" -t "paulojustinosilvadocker/web-scraping:latest" .``
	- _paulojustinosilvadocker_ is my Docker ID and _web-scraping_ is my repository
- login (enter your ID and password): ``docker login``
- push: ``docker push paulojustinosilvadocker/web-scraping:latest``
- My Docker Hub: https://hub.docker.com/r/paulojustinosilvadocker/web-scraping


# Web Scraping - Em português =D

## Projeto

O projeto foi modelado com DDD (Domain Driven Design) para guiar a construção do projeto baseado no Domínio.
O domínio possui o conhecimento de como buscar as informações via Web Scraping no GitHub, lidar com requisições HTTP, paralelismo, etc.

**Divisão das camadas**:

- **Api**: Expõe na Web uma API que tem como entrada a URL a ser aplicada WebScraping. Se relaciona com a Application e delega a ela responsabilidade de executar a operação de WebScraping.
- **Application**: Camada controlodora responsável por receber pedidos de webscraping e delegar à camada superior - Domain. A Application não tem conhecimento de como aplicar WebScraping.
- **Domain**: Camada principal que possui conhecimento de como aplicar WebScraping.
- **Domain.Shared**: Concentra DTO - Data Transfer Objects compartilháveis em toda aplicação e objetos de configuração do sistema - appsettings.json
- **CrossCutting**: Camada transversal as outras. Concentra extensions e exceptions utilizadas em todo projeto.
- **CrossCutting.IoC**: Limitado a Dependency Injection. A IoC aqui (Inversion of Control) foi usado no sentido de apenas injetarmos o serviço no cliente, ao invés do próprio cliente procurar e construir o serviço que irá utilizar.
- **Infra.Data**:
	- Possui elementos relacionados a Dados. No nosso caso, ao serviço de Cache. 
	- Mas poderia estabelecer repositórios (de Cache, NoSQL, Banco relacional), ou até concentrar lógica de obter dados HTML (o ponto central do WebScraping)
	- Repositórios de Dados de HTML não foram criadas aqui, porque a complexidade de se lidar com recursividade, paralelismo e performance é alta, logo segregar dados HTML em outra camada aumentaria mais ainda a complexidade.

## Domínio em detalhes

Ao receber o request, o domínio instancia o HttpClient - responsável pelas requisições das páginas HTML do GitHub e Semaphore - responsável por controlar o acesso paralelo ao HttpClient.

O método `ProcessAsync` é chamado pela 1° vez para buscar o HTML da 1° página.
A lib `AngleSharp` é usada como HTML Parser (fantástica por sinal!) e por ela fazemos perguntas do tipo:

- O HTML recebido representa uma página do GitHub de listagem de diretórios?
- Ou representa o conteúdo (linhas) de um arquivo?

Com essa pergunta respondida, conseguimos determinar se:

- Chamamos o `ProcessAsync` recursivamente para carregar outra página de listagem de diretórios
- Ou se procuramos no DOM as informações de quantidade de linhas e tamanho (em bytes)

A medida que as páginas de conteúdo de arquivo são encontradas, armazena-se em `ConcurrentBag temporaryFiles` os arquivos encontrados (nome, linhas e tamanho em byte).
Fazer esse processo recursivamente e 1 a 1 é lento com força. Então, utilizamos ParallelForEach para dar um UP na recursividade.

Todavia, o GitHub controla o acesso de seus recursos com **Rate Limiting**, então se consumirmos muitas páginas em pouco tempo, o GitHub bloqueia o acesso com **429 Too Many Requests**.
Para contornar isso, usamos Semaphore, que limita a quantidade de tasks que utilizam um determinado recurso (no nosso caso, limita a quantidade de tasks que acessam o HttpClient).
