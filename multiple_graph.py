import psycopg2
import matplotlib.pyplot as plt
import matplotlib.colors as mcolors
import pandas as pd
import numpy as np
import os

plt.rcParams['font.family'] = 'Malgun Gothic'
plt.rcParams['axes.unicode_minus'] = False

host = '10.10.20.103'  # 데이터베이스 호스트 주소
database = 'franchise'  # 데이터베이스 이름
user = 'postgres'  # 데이터베이스 사용자 이름
password = '1234'  # 데이터베이스 비밀번호
port = 5432  # 포트번호

conn = psycopg2.connect(host=host, database=database, user=user, password=password, port=port)
cur = conn.cursor()



class Graph:

    # 도넛 모양으로 그래프 그려주는 함수
    def create_donut_chart(self, gu, dong, todo_cnt: list):
        sizes = todo_cnt
        labels = ['10대', '20대', '30대', '기타']

        # 색 설정
        colors = ['#ff9999', '#66b3ff', '#99ff99', '#ffcc99', '#c2c2f0']
        # colors_ = ['#0F9B58', '#0FBC74', '#53B83A', '#3EC56B', '#1AA867', '#0FAF52', '#0FAF6B', '#53AF37']
        olive_color = ['#A4CD4A', '#FFC300', '#89CFF0', '#800080', '#AA0000']
        pastel_color_ = ['#B2F2BB', '#FFF5B1', '#AEDFF7', '#AFEEF2', '#FFDAC1']
        # pastel_color = ['#B2E09B', '#C1E5A9', '#AEDF93', '#BCE6B4', '#C8EDC7']

        # 비율에 따라 도넛 모양으로 그래프 그리기
        plt.pie(sizes, labels=labels, colors=colors, autopct='%1.1f%%', startangle=90, wedgeprops=dict(width=0.4))

        # 도넛 모양으로 그래프 그리기
        centre_circle = plt.Circle((0, 0), 0.70, fc='white')
        fig = plt.gcf()
        fig.gca().add_artist(centre_circle)

        # 모두 동일한 비율로 그리기
        plt.axis('equal')

        # 제목
        plt.title(f"{gu} {dong} 인구 비율")
        plt.show()  # 그래프 띄우기

        # 그래프 저장(show와 동시에 못 사용함)
        # save_graph(gu, dong, 'sample_folder')

    # 행정동별 타겟인구수
    def TARGET_POPULATION_ADMINISTRATIVE_DONG(self):
        query = f"SELECT * FROM public.\"TB_POPULATION\" ;"
        cur.execute(query)
        data = cur.fetchall()

        gwangju_data = {'광산구': [], '남구': [], '동구': [], '북구': [], '서구': []}
        for d in data:
            if d[2] not in gwangju_data.values():
                if d[1] == '광산구':
                    gwangju_data['광산구'].append(d[2])
                if d[1] == '남구':
                    gwangju_data['남구'].append(d[2])
                if d[1] == '동구':
                    gwangju_data['동구'].append(d[2])
                if d[1] == '북구':
                    gwangju_data['북구'].append(d[2])
                if d[1] == '서구':
                    gwangju_data['서구'].append(d[2])
        print(gwangju_data)
        keys = list(gwangju_data.keys())

        for i in gwangju_data:
            for j in gwangju_data[i]:
                print(i, j)
                return_data_by_dong(i, j)

    # 행정동별 인구수 리턴값
    def return_data_by_dong(gu, dong):
        query = f"SELECT * FROM public.\"TB_POPULATION\" WHERE \"POP_GU\" = '{gu}' AND \"POP_DONG\" = '{dong}';"
        print(query)
        cur.execute(query)
        data = cur.fetchall()
        print(data)
        data = data[0]
        id = data[0]
        gu_ = data[1]
        dong_ = data[2]
        teen = data[3] + data[4]
        twenty = data[5] + data[6]
        thirty = data[7] + data[8]
        etc_ = data[9]
        print(id, gu_, dong_, teen, twenty, thirty, etc_)
        create_donut_chart(gu=gu_, dong=dong_, todo_cnt=[teen, twenty, thirty, etc_])

    # 행정동별 월평균추정매출과 경쟁업체
    def AVERAGE_MONTHLY_ESTIMATED_SALES_COMPETITORS(self):

        sql_income = "select * from \"TB_SALES\" where \"SALES_GU\" = '북구'"
        cur.execute(sql_income)
        datas = cur.fetchall()

        dong_list = list()
        income_list = list()
        compete_list = list()
        for data in datas:
            dong = data[2]
            income = data[3]
            compete = data[4]

            if income != 0:
                dong_list.append(dong)
                income_list.append(income)
                compete_list.append(compete)

        x_value = np.array(dong_list)
        y1 = np.array(income_list)
        y2 = np.array(compete_list)

        fig, ax1 = plt.subplots()

        ax1.plot(x_value, y1, '-o', color='black', markersize=5, linewidth=3, alpha=0.7, label='월평균추정매출')
        ax1.set_xlabel('행정동')
        ax1.set_ylabel('행정동별 월평균추정매출')
        ax1.tick_params(axis='both', direction='in')

        ax2 = ax1.twinx()
        ax2.bar(x_value, y2, color='deeppink', label='경쟁업체 수', alpha=0.7, width=0.7)
        ax2.set_ylabel('동일업종 경쟁업체')
        ax2.tick_params(axis='y', direction='in')

        ax1.set_zorder(ax2.get_zorder() + 10)
        ax1.patch.set_visible(False)

        ax1.legend(loc='upper left')
        ax2.legend(loc='upper right')

        plt.title("행정동별 월평균추정매출과 경쟁업체")
        plt.show()

    # 행정동별 동일업종 월평균추정매출
    def AVERAGE_MONTHLY_ESTIMATED_SALES_SAME_INDUSTRY(self):
        """행정동별 동일업종 월평균추정매출"""
        sql_dong = "select * from \"TB_SALES\" where \"SALES_GU\" = '광산구' order by \"SALES_DONG\" asc"
        cur.execute(sql_dong)
        dong_datas = cur.fetchall()

        sql_pop = "select * from \"TB_POPULATION\" where \"POP_GU\" = '광산구' order by \"POP_DONG\" asc"
        cur.execute(sql_pop)
        pop_datas = cur.fetchall()

        dong_list = list()
        income_list = list()
        for data in dong_datas:
            dong = data[2]
            income = data[3]
            if income == 0:
                pass
            else:
                dong_list.append(dong)
                income_list.append(income)

        target_list = list()
        etc_list = list()
        for data in pop_datas:
            dong = data[2]
            target_age = sum(data[3:9])
            etc_age = data[-1]
            if dong not in dong_list:
                pass
            else:
                target_list.append(target_age)
                etc_list.append(etc_age)

        x = np.array(dong_list)
        bar = np.array(income_list)
        line = np.array(target_list)

        plt.rcParams['figure.figsize'] = (4, 3)
        plt.rcParams['font.size'] = 12

        x = np.array(dong_list)
        y1 = np.array(income_list)
        y2 = np.array(target_list)

        fig, ax1 = plt.subplots()

        ax1.plot(x, y1, '-s', color='green', markersize=7, linewidth=5, alpha=0.7, label='Price')
        ax1.set_xlabel('행정동')
        ax1.set_ylabel('동일업종 월평균추정매출')
        ax1.tick_params(axis='both', direction='in')

        ax2 = ax1.twinx()
        ax2.bar(x, y2, color='deeppink', label='Demand', alpha=0.7, width=0.7)
        ax2.set_ylabel("행정동별 1030 인구")
        ax2.tick_params(axis='y', direction='in')

        plt.show()

    # 다중이용시설 그래프
    def MULTI_USE_FACILITY_GRAPH(self):
        # 색상 설정
        color_list = ['#FFD1DC', '#AEDFF7', '#DCAEFE', '#B2F2BB', '#FFAD9F', '#FFDAC1', '#FFF5B1', '#AFEEF2',
                      '#FFE5B4', '#CBA3FF', '#FFB3BA', '#BAE1FF', '#FFE0B5', '#D1A3FF', '#A8FFD9']

        # 테이블 불러오기
        qurey = "select \"GU_NAME\", \"H_DONG_NAME\" from \"TB_DONG\""
        cur.execute(qurey)
        dong_datas = cur.fetchall()

        facilty_dict = dict()
        for data in dong_datas:
            gu = data[0]
            dong = data[1]
            if not gu in facilty_dict.keys():
                facilty_dict[gu] = []
            if dong not in facilty_dict[gu]:
                facilty_dict[gu].append(dong)
        print(facilty_dict)

        gu_list = list(facilty_dict.keys())
        for gu in gu_list:
            for dong in facilty_dict[gu]:
                sql_facility = f"select \"FACILITY_TYPE\", count(\"FACILITY_DONG\") from \"TB_FACILITY\" where \"FACILITY_GU\" = '{gu}' and \"FACILITY_DONG\" = '{dong}' group by \"FACILITY_TYPE\" "
                print(sql_facility)
                cur.execute(sql_facility)
                datas = cur.fetchall()

                # 다중이용시설, 값 담는 리스트
                facility_list = list()
                count_list = list()
                # 데이터 리스트에 추가하기
                for data in datas:
                    # print(data)
                    facility_type, facility_count = data[0], data[1]
                    facility_list.append(facility_type)
                    count_list.append(facility_count)
                print(facility_list, count_list)



                x_lables = np.arange(len(facility_list))
                facility = facility_list
                cnt = count_list

                # 제목 설정
                plt.title(f"{gu} {dong} 다중이용시설")

                # 그래프 색상 적용
                plt.bar(x_lables, cnt, color=color_list[:len(facility_list)])

                # 그래프에 값 넣기
                for i in range(len(cnt)):
                    plt.text(x_lables[i], cnt[i] - 0.5, str(cnt[i]), ha='center', fontsize=10, color='black')

                # 그래프 그리기
                # y축 값 설정
                if max(cnt) > 20:
                    plt.yticks(np.arange(0, max(cnt) + 1, 5))  # 5 단위로 나눔
                else:
                    plt.yticks(np.arange(0, max(cnt) + 1, 1))  # 1 단위로 나눔
                plt.xticks(x_lables, facility)
                plt.xticks(rotation=30)  # x 스틱 회전

                # plt.show()
                # plt.yticks(np.arange(min(cnt), max(cnt) + 1, step=1))

                plt.show()

    # 행정동별 면적 범위별 평균 보증금과 임대료
    def AVERAGE_WARRANTY_AREA_RANGE(self):
        sql_gu = 'select distinct "H_DONG_NAME", "GU_NAME" from "TB_DONG"'
        cur.execute(sql_gu)
        datas = cur.fetchall()

        for data in datas:
            dong_name = data[0]
            gu_name = data[1]
            sql_deal = f"select * from \"TB_DEAL\"  where \"DEAL_DONG\" = '{dong_name}' and \"DEAL_TYPE\" = '월세'"
            cur.execute(sql_deal)
            deal_datas = cur.fetchall()

            area_list = ['105 ~ 120', "121 ~ 135", '136 ~ 150', '151 ~ 165', '166 ~ 180', '181 ~ 200']
            deposit_1 = list()
            deposit_2 = list()
            deposit_3 = list()
            deposit_4 = list()
            deposit_5 = list()
            deposit_6 = list()

            rent_1 = list()
            rent_2 = list()
            rent_3 = list()
            rent_4 = list()
            rent_5 = list()
            rent_6 = list()

            for deal_data in deal_datas:
                # dong = data[4]
                deposit = deal_data[-4]
                rent = deal_data[-3]
                area = deal_data[-1]

                if rent != None:
                    if 105 <= area < 121:
                        rent_1.append(rent)
                        deposit_1.append(deposit)
                    elif 121 <= area < 136:
                        rent_2.append(rent)
                        deposit_2.append(deposit)
                    elif 135 <= area < 151:
                        rent_3.append(rent)
                        deposit_3.append(deposit)
                    elif 150 <= area < 166:
                        rent_4.append(rent)
                        deposit_4.append(deposit)
                    elif 166 <= area < 181:
                        rent_5.append(rent)
                        deposit_5.append(deposit)
                    elif 180 <= area < 201:
                        rent_6.append(rent)
                        deposit_6.append(deposit)

            avr_deposit = [self.calculate_average(deposit_list) for deposit_list in
                           [deposit_1, deposit_2, deposit_3, deposit_4, deposit_5, deposit_6]]
            avr_rent = [self.calculate_average(rent_list) for rent_list in
                        [rent_1, rent_2, rent_3, rent_4, rent_5, rent_6]]

            avr_deposit = [0 if np.isnan(value) else value for value in avr_deposit]
            avr_rent = [0 if np.isnan(value) else value for value in avr_rent]

            fig, ax1 = plt.subplots()

            ax1.plot(area_list, avr_deposit, '-o', color='#387D32', markersize=5, linewidth=3, alpha=0.7, label='평균 보증금')
            ax1.set_xlabel('면적')
            ax1.set_ylabel('범위별 평균 보증금')

            ax2 = ax1.twinx()

            # 그래프 색상 적용
            bar_color = '#A4CD4A'
            line_color = '#387D32'  # 진한 녹색
            ax2.bar(area_list, avr_rent, color=bar_color, label='평균 임대료', alpha=0.7, width=0.7)
            # ax2.bar(area_list, avr_rent, color=color_list[:len(datas)], label='평균 임대료', alpha=0.7, width=0.7)
            for i, value in enumerate(avr_rent):
                # ax2.bar(area_list[i], value, color=color_list[i], label=area_list[i], alpha=0.7, width=0.7)
                if round(avr_rent[i]) != 0:
                    plt.text(area_list[i], avr_rent[i] - 10, str(round(avr_rent[i])), ha='center', fontsize=11,
                             color='black', fontweight='bold')

            # ax2.bar(area_list, avr_rent, color='deeppink', label='평균 임대료', alpha=0.7, width=0.7)
            ax2.set_ylabel('범위별 평균 임대료')

            ax1.set_zorder(ax2.get_zorder() + 10)
            ax1.patch.set_visible(False)

            ax1.legend(loc='upper left')
            ax2.legend(loc='lower right')

            # yticks 설정
            if max(avr_deposit) == 0:
                ax1.set_yticks(np.arange(0, 1000, step=100))
            if max(avr_rent) == 0:
                ax2.set_yticks(np.arange(0, 1000, step=100))


            plt.title(f"{gu_name} {dong_name} 면적 범위별 평균 보증금과 임대료")
            self.save_graph(gu=gu_name, dong=dong_name, folder="구동별 보증금임대료")
            plt.show()

    # 그래프 저장하는 함수
    def SAVE_GRAPH(self, gu, dong, folder):
        """

        :param gu: 구 이름
        :param dong: 동 이름
        :param folder: 저장할 폴더
        :return:
        """
        # 그래프 저장
        # 현재 경로를 얻습니다.
        current_path = os.getcwd()

        # 현재 경로 내에 'facility_pic' 폴더를 생성
        graph_folder = os.path.join(current_path, f'{folder}')

        # 해당 폴더가 없으면 생성
        if not os.path.exists(graph_folder):
            os.makedirs(graph_folder)

        # 그림을 저장할 전체 경로를 설정
        fig = plt.gcf()
        save_path = os.path.join(graph_folder, f"{gu}_{dong}")
        fig.savefig(save_path)
        plt.close(fig)

    def calculate_average(self, data_list):
        """평균값 반환하는 함수"""
        valid_data = [value for value in data_list if value is not None and not np.isnan(value)]
        return np.mean(valid_data)

    # 구별 1030 인구대비 월평균추정매출
    def MONTHLY_SALES_RELATIVE_POPULATION_DISTINCTION(self):
        gu_list = ['광산구', '북구', '남구', '동구', '서구']
        for gu_name in gu_list:
            sql_dong = f"select * from \"TB_SALES\" where \"SALES_GU\" = '{gu_name}' order by \"SALES_DONG\" asc"
            cur.execute(sql_dong)
            dong_datas = cur.fetchall()

            sql_pop = f"select * from \"TB_POPULATION\" where \"POP_GU\" = '{gu_name}' order by \"POP_DONG\" asc"
            cur.execute(sql_pop)
            pop_datas = cur.fetchall()

            dong_list = list()
            income_list = list()
            for data in dong_datas:
                dong = data[2]
                income = data[3]
                if income == 0:
                    pass
                else:
                    dong_list.append(dong)
                    income_list.append(income)

            target_list = list()
            etc_list = list()
            for data in pop_datas:
                dong = data[2]
                target_age = sum(data[3:9])
                etc_age = data[-1]
                if dong not in dong_list:
                    pass
                else:
                    target_list.append(target_age)
                    etc_list.append(etc_age)

            x = np.array(dong_list)
            bar = np.array(income_list)
            line = np.array(target_list)

            plt.rcParams['figure.figsize'] = (4, 3)
            plt.rcParams['font.size'] = 12

            x = np.array(dong_list)
            y1 = np.array(income_list)
            y2 = np.array(target_list)

            # 화면 크기 조정
            # fig, ax1 = plt.subplots()
            fig, ax1 = plt.subplots(figsize=(9, 5))

            # 색상 설정
            color_list = ['#FFD1DC', '#AEDFF7', '#DCAEFE', '#B2F2BB', '#FFAD9F', '#FFDAC1', '#FFF5B1', '#AFEEF2',
                          '#FFE5B4', '#CBA3FF', '#FFB3BA', '#BAE1FF', '#FFE0B5', '#D1A3FF', '#A8FFD9']


            ax1.plot(x, y1, '-s', color='#2E4053', markersize=7, linewidth=4, alpha=0.7, label='Price') # 검정
            # ax1.plot(x, y1, '-s', color='#387D32', markersize=7, linewidth=5, alpha=0.7, label='Price') # 녹색
            ax1.set_xlabel('행정동')
            ax1.set_ylabel('동일업종 월평균추정매출')
            ax1.tick_params(axis='both', direction='in')

            ax1.set_xticks(np.arange(len(dong_list)))  # 틱의 위치 설정
            ax1.set_xticklabels(dong_list, rotation=45)  # 회전을 적용하여 x-axis 레이블을 설정

            ax2 = ax1.twinx()
            ax2.set_ylabel('10대 ~ 30대 인구수')

            bar_color = '#A4CD4A'
            line_color = '#387D32'  # 진한 녹색

            for i, value in enumerate(y2):
                ax2.bar(x[i], y2[i], color=color_list[i % len(color_list)], alpha=0.7, width=0.7) # 파스텔
                # ax2.bar(x[i], y2[i], color='#A4CD4A', alpha=0.7, width=0.7) # 녹색

                if round(y2[i]) != 0:
                    plt.text(x[i], y2[i] + 5, str(round(y2[i])),
                             ha='center', fontsize=11, color='black',
                             fontweight='bold', zorder=3)  # y 위치를 조정하고 zorder 추가
            ax1.set_zorder(ax2.get_zorder() + 10)
            ax1.patch.set_visible(False)
            plt.xticks(rotation=45)  # x 스틱 회전
            ax2.tick_params(axis='y', direction='in')

            plt.title(f'{gu_name} 1030 인구대비 월평균추정매출')

            plt.show()

    # 행정동별 월평균추정매출
    def MONTHLY_AVERAGE_ESTIMATED_SALES_ADMINISTRATIVE_DISTRICT(self):
        gu_list = ['광산구', '북구', '남구', '서구', '동구']
        for gu_name in gu_list:
            sql_income = f"select * from \"TB_SALES\" where \"SALES_GU\" = '{gu_name}'"
            cur.execute(sql_income)
            datas = cur.fetchall()

            dong_list = list()
            income_list = list()
            compete_list = list()
            for data in datas:
                dong = data[2]
                income = data[3]
                compete = data[4]

                if income != 0:
                    dong_list.append(dong)
                    income_list.append(income)
                    compete_list.append(compete)

            x_value = np.array(dong_list)
            y1 = np.array(income_list)
            y2 = np.array(compete_list)

            # fig, ax1 = plt.subplots()
            fig, ax1 = plt.subplots(figsize=(9, 5))

            ax1.plot(x_value, y1, '-o', color='#2E4053', markersize=5, linewidth=3, alpha=0.7, label='월평균추정매출')
            ax1.set_xlabel('행정동')
            ax1.set_ylabel('행정동별 월평균추정매출')
            ax1.tick_params(axis='both', direction='in')

            ax2 = ax1.twinx()

            # 색상 설정
            color_list = ['#FFD1DC', '#AEDFF7', '#DCAEFE', '#B2F2BB', '#FFAD9F', '#FFDAC1', '#FFF5B1', '#AFEEF2',
                          '#FFE5B4', '#CBA3FF', '#FFB3BA', '#BAE1FF', '#FFE0B5', '#D1A3FF', '#A8FFD9']
            for i, value in enumerate(y2):
                label = '동일업종 경쟁업체' if i == 0 else None  # 첫 번째 바에만 라벨을 지정
                ax2.bar(x_value[i], y2[i], color=color_list[i % len(color_list)], alpha=0.7, width=0.7, label=label)


                if round(y2[i]) != 0:
                    plt.text(x_value[i], y2[i], str(round(y2[i])),
                             ha='center', fontsize=11, color='black',
                             fontweight='bold', zorder=2)  # y 위치를 조정하고 zorder 추가

            # ax2.bar(x_value, y2, color='deeppink', label='경쟁업체 수', alpha=0.7, width=0.7)
            ax2.set_ylabel('동일업종 경쟁업체')
            # ax2.tick_params(axis='y', direction='in')
            # x축 틱 위치와 레이블을 설정합니다.
            ax1.set_xticks(np.arange(len(x_value)))
            ax1.set_xticklabels(x_value, rotation=45)

            ax2.tick_params(axis='y', direction='in')
            # plt.xticks(rotation=45)  # x 스틱 회전

            ax1.set_zorder(ax2.get_zorder() + 10)
            ax1.patch.set_visible(False)

            ax1.legend(loc='upper left')
            ax2.legend(loc='upper right')

            plt.title(f"{gu_name} 월평균추정매출과 경쟁업체")

            # 그래프 저장
            # 현재 경로를 얻습니다.
            current_path = os.getcwd()

            # 현재 경로 내에 'facility_pic' 폴더를 생성
            graph_folder = os.path.join(current_path, '월평균추정매출과 경쟁업체')

            # 해당 폴더가 없으면 생성
            if not os.path.exists(graph_folder):
                os.makedirs(graph_folder)

            # 그림을 저장할 전체 경로를 설정
            fig = plt.gcf()
            save_path = os.path.join(graph_folder, f"{gu_name} 월평균추정매출과 경쟁업체")
            fig.savefig(save_path)
            plt.close(fig)
            # plt.show()

            plt.show()

    # 정규화 전 분포도 그리기
    def PRE_NORMALIZATION_GRAPH(self):

        df = pd.read_csv('정규화전.csv')

        areas = df['구동'].tolist()

        # 나머지 열은 각 카테고리에 해당하는 값
        categories = df.columns[1:].tolist()

        # 컬러맵 설정
        colormap = plt.cm.tab20
        color_list = [colormap(i) for i in range(len(categories))]
        color_list
        # colors = ['red', 'blue', 'green']

        fig, ax = plt.subplots(figsize=(16, 10))

        # 각 지역에 대한 산점도 그리기
        for idx, category in enumerate(categories):
            y_values = df[category].tolist()
            ax.scatter(areas, y_values, color=color_list[idx], label=category)

        ax.set_ylabel('값')
        ax.set_xlabel('지역명')
        ax.set_title('지역별 카테고리 값(정규화 전)')
        ax.legend()

        plt.xticks(rotation=45)
        plt.tight_layout()
        plt.show()

    # 정규화 이후 분포도 그리기
    def GRAPHS_NORMALIZATION_GRAPH(self):

        df2 = pd.read_csv('정규화후.csv')
        # df2
        areas = df2['구동'].tolist()

        # 나머지 열은 각 카테고리에 해당하는 값
        categories2 = df2.columns[1:].tolist()

        # # 컬러맵 설정
        colormap = plt.cm.tab20
        color_list = [colormap(i) for i in range(len(categories2))]
        # # colors = ['red', 'blue', 'green']

        fig, ax = plt.subplots(figsize=(16, 10))

        # 각 지역에 대한 산점도 그리기
        for idx, category in enumerate(categories2):
            y_values = df2[category].tolist()
            ax.scatter(areas, y_values, color=color_list[idx], label=category)

        ax.set_ylabel('값')
        ax.set_xlabel('지역명')
        ax.set_title('지역별 카테고리 값(정규화 후)')
        ax.legend()

        plt.xticks(rotation=45)
        plt.tight_layout()
        plt.show()


if __name__ == '__main__':
    Graph().MONTHLY_AVERAGE_ESTIMATED_SALES_ADMINISTRATIVE_DISTRICT()
