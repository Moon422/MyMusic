BACKEND=Backend
FRONTEND=Frontend
CC=dotnet

wb:
	$(CC) watch run --project $(BACKEND) 

rb:
	$(CC) run --project $(BACKEND) 
