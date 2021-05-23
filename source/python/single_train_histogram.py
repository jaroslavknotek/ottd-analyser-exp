import pandas as pd
import matplotlib.pyplot as plt
import sys
import os
import numpy as np

def print_histogram(data_record, train_id):
    key = data_record['segmentId']
    data= data_record[1]
    
    med = np.median(data)
    plt.hist(data,bins=50)
    plt.axvline(x=med,color = 'red')
    low = np.floor(med*.9)
    plt.axvline(x=low,color = 'green')
    grt = np.ceil(med*1.1)
    plt.axvline(x=grt,color = 'green')
    plt.savefig("../../out/train-delay_{}_{}.png".format(train_id, key))
    plt.clf()
        
def pandas_printout(df,train_id):
    df['datediff'] = df['datetime'].shift(periods=-1)- df['datetime']
    df['datediff'] = df['datediff'].dt.days
    df['segmentId'] = df['orderNumberCurrent'].astype(str).shift(periods=-1) + "_" + df['orderNumberCurrent'].astype(str)
    
    df.drop(df.tail(1).index,inplace=True)
    groups = df.groupby(['segmentId'])['datediff'].apply(list).reset_index()
    groups.apply(func=lambda x:print_histogram(x,train_id),axis = 1)

def print_all(data_directory):
    for filename in os.listdir(data_directory):
        if filename.endswith(".log"):
            file = os.path.join(data_directory,filename)
            print(file)
            df = pd.read_json(file,dtype=False, lines=True)
            train_id = filename.replace(".log","")
            pandas_printout(df, train_id)
        else:
            continue

data_path = sys.argv[1].replace('\\','/')
print_all(data_path)