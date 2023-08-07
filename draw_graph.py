import os
import matplotlib.pyplot as plt

# 현재 경로를 얻습니다.
current_path = os.getcwd()

# 현재 경로 내에 'graph_pic' 폴더를 만듭니다.
graph_folder = os.path.join(current_path, 'graph_pic')

# 해당 폴더가 없으면 생성합니다.
if not os.path.exists(graph_folder):
    os.makedirs(graph_folder)

# 그림을 저장할 전체 경로를 설정합니다.
save_path = os.path.join(graph_folder, 'sample.png')

fig, ax = plt.subplots(nrows=1, ncols=1)  # create figure & 1 axis
ax.plot([0,1,2], [10,20,3])
fig.savefig(save_path)   # save the figure to file
plt.close(fig)
