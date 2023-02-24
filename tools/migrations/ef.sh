cd ../.. || exit
cd src/Commands || exit

echo "Migration name:"
read -r migrationName

dotnet ef migrations add "$migrationName" -o Persistence/Migrations
dotnet ef database update
