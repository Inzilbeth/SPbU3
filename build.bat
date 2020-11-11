FOR /D %%n IN (Homework*) DO ( 
    dotnet build %%n/Task1/Task1.sln
)