BACKEND=Backend
FRONTEND=Frontend
CC=dotnet

wb:
	$(CC) watch run --project $(BACKEND) 

rb:
	$(CC) run --project $(BACKEND) 

db:
	$(CC) ef migrations add "$(msg)" --project $(BACKEND)
	$(CC) ef database update --project $(BACKEND) 
