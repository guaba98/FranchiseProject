import pandas as pd
import matplotlib.pyplot as plt
import pandas as pd
import psycopg2
import os


# 글씨체 설정
plt.rcParams['axes.unicode_minus'] =False
plt.rcParams['font.family'] ='Malgun Gothic'

# db 호출
host = '10.10.20.103'  # 데이터베이스 호스트 주소
database = 'franchise'  # 데이터베이스 이름
user = 'postgres'  # 데이터베이스 사용자 이름
password = '1234'  # 데이터베이스 비밀번호
port = 5432  # 포트번호

conn = psycopg2.connect(host=host, database=database, user=user, password=password, port=port)
cur = conn.cursor()


def create_donut_chart(dong, todo_cnt: list):
    sizes = todo_cnt
    labels = ['10대', '20대', '30대', '기타']
    colors = ['#ff9999', '#66b3ff', '#99ff99', '#ffcc99', '#c2c2f0']
    colors_ = ['#0F9B58', '#0FBC74', '#53B83A', '#3EC56B', '#1AA867', '#0FAF52', '#0FAF6B', '#53AF37']
    olive_color = ['#A4CD4A', '#FFC300', '#89CFF0', '#800080', '#AA0000']

    # 비율에 따라 도넛 모양으로 그래프 그리기

    plt.pie(sizes, labels=labels, colors=olive_color, autopct='%1.1f%%', startangle=90, wedgeprops=dict(width=0.4))

    # 도넛 모양으로 그래프 그리기
    centre_circle = plt.Circle((0, 0), 0.70, fc='white')
    fig = plt.gcf()
    fig.gca().add_artist(centre_circle)

    # 모두 동일한 비율로 그리기
    plt.axis('equal')
    plt.title(f"{dong} 인구 비율")



    ### 이미지 저장
    # 현재 경로를 얻습니다.
    current_path = os.getcwd()

    # 현재 경로 내에 'graph_pic' 폴더를 만듭니다.
    graph_folder = os.path.join(current_path, 'graph_pic')

    # 해당 폴더가 없으면 생성합니다.
    if not os.path.exists(graph_folder):
        os.makedirs(graph_folder)

    # 그림을 저장할 전체 경로를 설정합니다.
    save_path = os.path.join(graph_folder, f'{dong}_인구비율.png')
    fig.savefig(save_path)
    plt.close(fig)
    plt.show()


def return_specific_data():
    query = f"SELECT * FROM public.\"TB_POPULATION\" ;"
    cur.execute(query)
    data = cur.fetchall()

    gwangju_data = dict()
    for d in data:
        if d[1] not in gwangju_data.keys():
            gwangju_data[d[1]] = []
        if d[2] not in gwangju_data[d[1]]:
            gwangju_data[d[1]].append(d[2])
    dong_list = list(gwangju_data.values())
    dong_list_ = []
    for list_ in dong_list:
        if type(list_) == list:
            dong_list_.extend(list_)
    print(dong_list_)
    for dong in dong_list_:
        return_data_by_dong(dong)

def return_data_by_dong(dong):
    query = f"SELECT * FROM public.\"TB_POPULATION\" WHERE \"POP_DONG\" = '{dong}';"
    print(query)
    cur.execute(query)
    data = cur.fetchall()
    print(data)
    data = data[0]
    id = data[0]
    gu = data[1]
    dong = data[2]
    teen = data[3] + data[4]
    twenty = data[5] + data[6]
    thirty = data[7] + data[8]
    etc_ = data[9]
    create_donut_chart(dong=dong, todo_cnt=[teen, twenty, thirty, etc_])


    conn.commit()

return_specific_data()

cur.close()
conn.close()
