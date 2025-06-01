# script.py
from TTS.api import TTS
import os

def main():
    text = "123"  
    model_name = "tts_models/ru/ljspeech/tacotron2-DDC"  # Модель для русского языка

 
    script_dir = os.path.dirname(os.path.abspath(__file__))
    output_path = os.path.join(script_dir, "output.wav")  

    print(f"Сохранение аудио в: {output_path}") # Вывод для отладки

    try:
        
        print("Инициализация TTS...")
        tts = TTS(model_name=model_name, progress_bar=False, gpu=False)
        print("TTS инициализирован.")
    except Exception as e:
        print(f"Ошибка инициализации TTS: {e}")
       
        return

    try:
        
        print(f"Синтез речи для текста: '{text}'...")
        tts.tts_to_file(text=text, file_path=output_path)
        print(f"Файл успешно сохранен в: {output_path}")
    except Exception as e:
        print(f"Ошибка синтеза речи или сохранения файла: {e}")

if __name__ == "__main__":
    main()