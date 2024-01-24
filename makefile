BACKEND=Backend
FRONTEND=Frontend
CC=dotnet

wb:
	$(CC) watch run --project $(BACKEND) 

rb:
	$(CC) run --project $(BACKEND) 

wf:
	$(CC) watch run --project $(FRONTEND)

rf:
	$(CC) run --project $(FRONTEND)

db:
	$(CC) ef migrations add "$(msg)" --project $(BACKEND)
	$(CC) ef database update --project $(BACKEND) 
	git add $(BACKEND)/Migrations 
	git commit -m '$(msg)' 
	echo "migration complete and committed to git"
