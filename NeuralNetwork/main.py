import csv
from csv import reader
import nltk
import string
import pandas
import numpy as np
import matplotlib.pyplot as plt
from keras import backend
import tensorflow as tf
from tensorflow.keras.preprocessing.sequence import pad_sequences
from tensorflow.keras.layers import Embedding, LSTM, Dense, Bidirectional
from tensorflow.keras.preprocessing.text import Tokenizer
from tensorflow.keras.models import Sequential
from tensorflow.keras.optimizers import Adam

path = 'F:\\Magisterka\\SaveData.csv'
dataset = []
with open(path, 'r') as data_file: #Reading data.
    reader = reader(data_file)
    for row in reader:
        dataset.append(''.join(row))
#print(dataset)

tokenizer = Tokenizer(filters='\t\n') #Extracting unique node GUIDs.
tokenizer.fit_on_texts(dataset)
total_unique_words = len(tokenizer.word_index) + 1 
#print(total_unique_words)
#print(tokenizer.word_index)

input_sequences = [] #Creating a list of all node sequences.
for line in dataset:
    token_list = tokenizer.texts_to_sequences([line])[0]
    for i in range(1, len(token_list)): 
          n_gram_seqs = token_list[:i+1]
          input_sequences.append(n_gram_seqs)
#print(len(input_sequences))
#print(input_sequences)

max_seq_length = max([len(x) for x in input_sequences]) #Padding sequences.
input_seqs = np.array(pad_sequences(input_sequences, maxlen=max_seq_length, padding='pre'))
#print(max_seq_length)
#print(input_seqs[:5])
x_values, labels = input_seqs[:, :-1], input_seqs[:, -1]
y_values = tf.keras.utils.to_categorical(labels, num_classes=total_unique_words)
#print(x_values[:3])
#print(labels[:3])
backend.clear_session()
epochs = 200
model = tf.keras.Sequential([
tf.keras.layers.Embedding(input_dim = total_unique_words, output_dim=3, input_length=max_seq_length-1),
tf.keras.layers.Bidirectional(tf.keras.layers.LSTM(256, return_sequences=True)),
tf.keras.layers.Dropout(0.2), 
tf.keras.layers.Bidirectional(tf.keras.layers.LSTM(256)),
tf.keras.layers.Dropout(0.2),
tf.keras.layers.Dense(128, activation='relu'),
tf.keras.layers.Dense(total_unique_words , activation='softmax')])
model.compile(optimizer=Adam(learning_rate=0.001), loss='categorical_crossentropy', metrics=['accuracy'])
history = model.fit(x_values, y_values, epochs=epochs, validation_split=0.2, verbose=1, batch_size=256)
epochs_range = range(epochs)
accuracy = history.history['accuracy']
valid_accuracy = history.history['val_accuracy']

loss = history.history['loss']
valid_loss = history.history['val_loss']

plt.figure(figsize=(8, 8))
plt.subplot(1, 2, 1)
plt.plot(epochs_range, accuracy, label='Training Accuracy')
plt.plot(epochs_range, valid_accuracy, label='Validation Accuracy')
plt.legend(loc='lower right')
plt.title('Training and Validation Accuracy')

plt.subplot(1, 2, 2)
plt.plot(epochs_range, loss, label='Training Loss')
plt.plot(epochs_range, valid_loss, label='Validation Loss')
plt.legend(loc='upper right')
plt.title('Training and Validation Loss')
plt.show()

def prediction(seed_text, next_words): 
  for _ in range(next_words):
    token_list = tokenizer.texts_to_sequences([seed_text])[0]
  token_list = pad_sequences([token_list], maxlen=max_seq_length-1, padding='pre')
  predicted = np.argmax(model.predict(token_list, verbose=1), axis=-1)
  ouput_word = ""
  for word, index in tokenizer.word_index.items():
    if index == predicted:
      output_word = word
      break
  seed_text += ' '+output_word
  print(seed_text)
seed_phrase = "6ff9ed28-41ec-4fd2-b342-5b87f1ae0c5b 55c865ee-c650-4a5a-85e0-c64dd8a53247"
prediction(seed_phrase, 1)
def save_model(self, path):
  if self.model != None:
    self.model.save(path + '.h5')
model.save('model.h5')