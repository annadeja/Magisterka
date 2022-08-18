import sys
import json
import numpy as np
import tensorflow as tf
from tensorflow.keras.preprocessing.sequence import pad_sequences
from tensorflow.keras.preprocessing.text import Tokenizer, tokenizer_from_json

model_path = sys.argv[1]
model = tf.keras.models.load_model(model_path)
tokenizer = Tokenizer(filters='\t\n')
tokenizer_path = sys.argv[2]
with open(tokenizer_path) as file:
    data = json.load(file)
    tokenizer = tokenizer_from_json(data)
node_sequence = sys.argv[3]
ouput_guid = ""
if model != None:
    token_list = tokenizer.texts_to_sequences([node_sequence])[0]
    max_sequence_length = 37
    token_list = pad_sequences([token_list], maxlen=max_sequence_length-1, padding='pre')
    predicted = np.argmax(model.predict(token_list, verbose=1), axis=-1)
    for guid, index in tokenizer.word_index.items():
        if index == predicted:
            ouput_guid = guid
            break
print(ouput_guid)