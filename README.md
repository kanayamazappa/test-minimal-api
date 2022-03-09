# test-minimal-api

1 - Criar uma Solução
    dotnet new sln --name ServiceAdmProduct 
2 - Criar Projetos
    dotnet new web -o "1 - Service" -n "Service" -f "net6.0"
    dotnet new classlib -o "2 - Application" -n "Application" -f "netstandard2.1"
    dotnet new classlib -o "3 - Repository" -n "Repository" -f "netstandard2.1"
    dotnet new classlib -o "4 - Domain" -n "Domain" -f "netstandard2.1"
3 - Adicionar a Api a solução
    dotnet sln add '.\1 - Service\Service.csproj'
    dotnet sln add '.\2 - Application\Application.csproj'
    dotnet sln add '.\3 - Repository\Repository.csproj'
    dotnet sln add '.\4 - Domain\Domain.csproj'
4 - Adicionar referencias de camadas
    dotnet add ".\3 - Repository\Repository.csproj" reference ".\4 - Domain\Domain.csproj"
    dotnet add ".\2 - Application\Application.csproj" reference ".\4 - Domain\Domain.csproj"
    dotnet add ".\2 - Application\Application.csproj" reference ".\3 - Repository\Repository.csproj"
    dotnet add ".\1 - Service\Service.csproj" reference ".\4 - Domain\Domain.csproj"
    dotnet add ".\1 - Service\Service.csproj" reference ".\3 - Repository\Repository.csproj"
    dotnet add ".\1 - Service\Service.csproj" reference ".\2 - Application\Application.csproj"
