import mysql.connector

# MySQL 연결 설정
conn = mysql.connector.connect(
    host='210.119.12.54',
    port=3306,
    user='root',
    password='12345',
    database='miniproject'
)

cursor = conn.cursor()

# 데이터베이스에서 조회
query = "SELECT name FROM opencv"

cursor.execute(query)

# 커밋(insert)
# conn.commit()

# 데이터 출력
result = cursor.fetchall()
for row in result:
    print(row)

# 연결 종료
cursor.close()
conn.close()
