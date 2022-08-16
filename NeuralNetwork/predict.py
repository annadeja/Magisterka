import sys
import numpy as np
import json
import tensorflow as tf
from tensorflow.keras.preprocessing.text import Tokenizer, tokenizer_from_json
from tensorflow.keras.preprocessing.sequence import pad_sequences

model_path = sys.argv[1]
model = tf.keras.models.load_model(model_path)
tokenizer = Tokenizer(filters='\t\n')
tokenizer_path = sys.argv[2]
with open(tokenizer_path) as f:
    data = json.load(f)
    tokenizer = tokenizer_from_json(data)
text_input = sys.argv[3]
ouput_word = ""
if model != None:
    token_list = tokenizer.texts_to_sequences([text_input])[0]
    max_seq_length = 37
    token_list = pad_sequences([token_list], maxlen=max_seq_length-1, padding='pre')
    predicted = np.argmax(model.predict(token_list, verbose=1), axis=-1)
    for word, index in tokenizer.word_index.items():
        if index == predicted:
            output_word = word
            break
print(output_word)