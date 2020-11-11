FOR /D %%n IN (Homework*) DO ( 
    nuget restore %%n/Task1/Task1.sln
)