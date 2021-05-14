import pandas as pd
from itertools import islice, groupby
import matplotlib.pyplot as plt

data_path = '../my_data/logs.all'
data_path = '../my_data/logs.subset'
data_path = '../my_data/0.log'

def print_histogram(x):
        key = x['segmentId']
        data= x[1]
        plt.hist(data)
        plt.savefig("../out/train-delay_{}.png".format(key))
        plt.clf()
        
def pandas_printout(df):
    df['datediff'] = df['date'].shift(periods=-1) .astype(float)- df['date'].astype(float)
    df['segmentId'] = df['orderNumberCurrent'].shift(periods=-1).astype(str) + "_" + df['orderNumberCurrent'].astype(str)
    
    df.drop(df.tail(1).index,inplace=True)
    
    groups = df.groupby(['segmentId'])['datediff'].apply(list).reset_index()
    
    
    groups.apply(func=print_histogram,axis = 1)

df = pd.read_json(data_path,dtype=False, lines=True)

pandas_printout(df)
