import io
import os
import json
import numpy as np
import tensorflow as tf
import matplotlib.pyplot as plt
from csv import reader
from tensorflow.keras import regularizers, backend
from tensorflow.keras.optimizers import Adam
from tensorflow.keras.models import Sequential
from tensorflow.keras.preprocessing.text import Tokenizer
from tensorflow.keras.preprocessing.sequence import pad_sequences
from tensorflow.keras.layers import Embedding, LSTM, Dense, Bidirectional

##Zawiera wszystkie dane oraz funkcje niezbędne do wytrenowania specyficznej sieci neuronowej.
class NeuralNetwork(object):
  ##Konstruktor klasy.
  #@param self Obecny obiekt.
  #@param path Ścieżka do zestawu danych.
  def __init__(self, path):
      super().__init__()
      ##Przechowuje ścieżkę do danych na dysku.
      self.path = path
      ##Przechowuje wczytany zestaw danych.
      self.dataset = []
      ##Przechowuje cechy (kontekst) w celu treningu sieci.
      self.x_values = []
      ##Przechowuje klasy (określone GUID) w celu treningu sieci.
      self.y_values = []
      ##Przedstawis maksymalną długość danej sekwencji GUID.
      self.max_sequence_length = 0
      ##Przedstawa ilość unikatowych GUID.
      self.unique_guids = 0
      ##Wyznacza ilość epok treningu.
      self.epochs = 200
      ##Wyznacza wielkość pojedynczych próbek danych pobieranych podczas treningu.
      self.batch_size = 32
      ##Przechowuje model sieci neuronowej.
      self.model = None
      ##Przechowuje obiekt umożliwiający przetwarzanie danych tekstowych.
      self.tokenizer = None

  ##Czyta z dysku zestaw danych
  #@param self Obecny obiekt.
  def create_dataset(self):
    with open(self.path, 'r') as data_file:
      csv_reader = reader(data_file)
      for row in csv_reader:
        self.dataset.append(''.join(row))

  ##Przetwarza wczytany zestaw danych.
  #@param self Obecny obiekt.
  def process_dataset(self):
    self.tokenizer = Tokenizer(filters='\t\n') #Ekstrakcja wszystkich unikatowych GUID.
    self.tokenizer.fit_on_texts(self.dataset)
    self.unique_guids = len(self.tokenizer.word_index) + 1 

    tokenizer_json = self.tokenizer.to_json()
    with io.open('tokenizer.json', 'w', encoding='utf-8') as file:
        file.write(json.dumps(tokenizer_json, ensure_ascii=False))

    input_sequences = [] #Tworzenie listy wszystkich podsekwencji danej sekwencji GUID.
    for line in self.dataset:
        token_list = self.tokenizer.texts_to_sequences([line])[0]
        for i in range(1, len(token_list)): 
              n_grams = token_list[:i+1]
              input_sequences.append(n_grams)

    self.max_sequence_length = max([len(x) for x in input_sequences])
    #Dodawanie pustych wartości do podsekwencji w celu zrównania ich długości.
    padded_sequences = np.array(pad_sequences(input_sequences, maxlen=self.max_sequence_length, padding='pre'))

    self.x_values, labels = padded_sequences[:, :-1], padded_sequences[:, -1] #Tworzenie ostatecznych zestawów danych do treningu.
    self.y_values = tf.keras.utils.to_categorical(labels, num_classes=self.unique_guids)

  ##Tworzy obiekt modelu sieci neuronowej, jednocześnie określając jej strukturę.
  #@param self Obecny obiekt.
  def create_model(self):
    self.model = tf.keras.Sequential([
    tf.keras.layers.Embedding(input_dim = self.unique_guids, output_dim=3, input_length=self.max_sequence_length-1),
    tf.keras.layers.Bidirectional(tf.keras.layers.LSTM(512, return_sequences=True, kernel_regularizer=regularizers.l2(0.001))),
    tf.keras.layers.Dropout(0.2), 
    tf.keras.layers.Bidirectional(tf.keras.layers.LSTM(256, kernel_regularizer=regularizers.l2(0.001))),
    tf.keras.layers.Dropout(0.2),
    tf.keras.layers.Dense(128, activation='relu'),
    tf.keras.layers.Dense(self.unique_guids , activation='softmax')])

  ##Trenuje sieć neuronową oraz realizuje operacje wymagane do treningu.
  #@param self Obecny obiekt.
  def train_network(self):
    self.create_dataset()
    self.process_dataset()
    backend.clear_session()

    self.create_model()
    self.model.compile(optimizer=Adam(learning_rate=0.001), loss='categorical_crossentropy', metrics=['accuracy'])
    self.model.summary()

    history = self.model.fit(self.x_values, self.y_values, epochs=self.epochs, validation_split=0.2, verbose=1, batch_size=self.batch_size)
    self.plot_training_data(history)

  ##Przedstawia w formie wykresu dane dotyczące procesu trenowania modelu.
  #@param self Obecny obiekt.
  #@param history Dane dotyczące procesu trenowania.
  def plot_training_data(self, history):
    epochs_range = range(self.epochs)
    accuracy = history.history['accuracy']
    val_accuracy = history.history['val_accuracy']

    loss = history.history['loss']
    val_loss = history.history['val_loss']

    plt.figure(figsize=(8, 8))
    plt.subplot(1, 2, 1)
    plt.plot(epochs_range, accuracy, label='Training Accuracy')
    plt.plot(epochs_range, val_accuracy, label='Validation Accuracy')
    plt.legend(loc='lower right')
    plt.title('Training and Validation Accuracy')

    plt.subplot(1, 2, 2)
    plt.plot(epochs_range, loss, label='Training Loss')
    plt.plot(epochs_range, val_loss, label='Validation Loss')
    plt.legend(loc='upper right')
    plt.title('Training and Validation Loss')
    plt.show()

  ##Przewiduje następny GUID na podstawie sekwencji poprzednich.
  #@param self Obecny obiekt.
  #@param node_sequence Sekwencja GUID w postaci jednego tekstu.
  def prediction(self, node_sequence): 
    token_list = tokenizer.texts_to_sequences([node_sequence])[0]
    token_list = pad_sequences([token_list], maxlen=self.max_sequence_length-1, padding='pre')
    predicted = np.argmax(model.predict(token_list, verbose=1), axis=-1)
    ouput_guid = ""
    for guid, index in tokenizer.word_index.items():
      if index == predicted:
        output_guid = guid
        break
    print(ouput_guid)

  ##Zapisuje model do pliku.
  #@param self Obecny obiekt.
  #@param path Ścieżka do zapisu modelu.
  def save_model(self, path):
    if self.model != None:
      self.model.save(path + '.h5')

def main():
    print("Number of GPUs Available: ", len(tf.config.list_physical_devices('GPU')))
    neural_network = NeuralNetwork(os.getcwd() + "\\SaveData.csv")
    neural_network.train_network()
    neural_network.save_model("model")

if __name__ == "__main__":
    main()