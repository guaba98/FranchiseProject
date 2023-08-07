import json
import psycopg2

class DBconnect:
    def __init__(self):
        super().__init__()

        # query = f"SELECT * FROM public.\"TB_DEAL\" ;"
        # cur.execute(query)
        # data = cur.fetchall()

    def connect_db(self):
        host = '10.10.20.103'  # 데이터베이스 호스트 주소
        database = 'franchise'  # 데이터베이스 이름
        user = 'postgres'  # 데이터베이스 사용자 이름
        password = '1234'  # 데이터베이스 비밀번호
        port = 5432  # 포트번호

        self.conn = psycopg2.connect(host=host, database=database, user=user, password=password, port=port)
        self.cur = self.conn.cursor()

    def find_dong(self, gu_name):
        """구 이름을 받으면 행정동을 검색합니다"""
        query = f"select distinct \"H_DONG_NAME\" from public.\"TB_DONG\" where \"GU_NAME\" = '{gu_name}';"
        data_list = self.excuete_query(query)
        data_split_list = [n[0] for n in data_list]
        # json_data = json.dumps(data_list)
        return data_split_list

    def excuete_query(self, query):
        """조건에 따라 튜플에 담긴 데이터들이 리스트 안에 담겨 리턴됩니다."""
        tmp_list = list()
        self.connect_db()
        self.cur.execute(query)
        datas = self.cur.fetchall()

        for data in datas:
            tmp_list.append(data)
        return tmp_list

def another_function(arg1, arg2):
    # 여기에 원하는 코드를 작성
    return f"Arguments received: {arg1}, {arg2}"

if __name__ == '__main__':
    import sys
    if len(sys.argv) < 2:
        print("Not enough arguments!")
        sys.exit(1)

    action = sys.argv[1]
    db_obj = DBconnect()

    if action == "find_dong":
        gu_name = sys.argv[2]
        result = db_obj.find_dong(gu_name)
        print(result)
    elif action == "another_function":
        arg1 = sys.argv[2]
        arg2 = sys.argv[3]
        result = another_function(arg1, arg2)
        print(result)
    else:
        print(f"No action matched for {action}")