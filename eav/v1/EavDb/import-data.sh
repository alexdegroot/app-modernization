# Try 50 times to 
for i in {1..50};
do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d master -i /usr/src/app/Eav/EavModel.sql
    if [ $? -eq 0 ]
    then
        echo "setup.sql completed"
        break
    else
        echo "not ready yet... $i"
        sleep 1
    fi
done