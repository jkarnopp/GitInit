dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet restore
dotnet ef

cd ..\GitInitTest.Data\
dotnet ef migrations add CreateDb
dotnet ef database update
cd ..\GitInitTest.Site\

npm install

