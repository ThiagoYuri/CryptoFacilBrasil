### In Folder "/CryptoFacilBrasil": 
    dotnet ef migrations add "First_Migration" --project ".\4- Infrastructure\Infrastructure.Entityframework" --startup-project ".\1- Application\Application.CryptoFacilBrasil"
    dotnet ef database update --project ".\4- Infrastructure\Infrastructure.Entityframework" --startup-project ".\1- Application\Application.CryptoFacilBrasil"