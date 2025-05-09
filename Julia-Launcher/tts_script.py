from TTS.api import TTS

def main():
    text = "Привет мир!"  # Фиксированный текст
    model_name = "tts_models/ru/ljspeech/tacotron2-DDC"  # Модель для русского языка
    output_path = "output.wav"  # Имя выходного файла

    # Инициализация TTS
    tts = TTS(model_name=model_name)

    # Синтез речи и сохранение в файл
    tts.tts_to_file(text=text, file_path=output_path)

if __name__ == "__main__":
    main()