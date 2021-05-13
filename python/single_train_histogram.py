import pandas as pd
from itertools import islice, groupby
import matplotlib.pyplot as plt

data_path = '../my_data/logs.all'
data_path = '../my_data/logs.subset'
data_path = '../my_data/0.log'

df = pd.read_json(data_path, lines=True)

# islice because I want to debug it first
subsequent = islice(zip(df.iterrows(), islice(df.iterrows(),1,None)),500)
only_rows = [ (row_current[1], row_next[1]) for row_current,row_next in subsequent ]
diffs = [    (int(rn['date']) - int(rc['date']), str(rc['orderNumberCurrent']) + '_' + str(rn['orderNumberCurrent']) ) for rc,rn in only_rows ] 

data = sorted(diffs, key=lambda x: x[1])
groups = groupby(data, key=lambda x: x[1])

for key, group in groups:
    data = [x for (x,_) in list(group)]
    plt.hist(data)
    plt.savefig("plot_{}.png".format(key))
    plt.clf()




