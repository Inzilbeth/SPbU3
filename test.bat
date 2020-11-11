FOR /D %%n IN (Homework*) DO ( 
    dotnet test %%n/Task1/Task1.sln
)