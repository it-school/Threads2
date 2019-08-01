using System;
using System.Threading;

namespace Threads2
{
    class Program
    {
        static int result = 0;

        static void Main(string[] args)
        {
            // BGthread();
            // SendDataToSecondaryThread();
            // CallBackExample();
        }

        static void BGthread()
        {
            Worker w0 = new Worker(100);
            ThreadStart t0;
            t0 = new ThreadStart(w0.DoItEasy);
            Thread th0;
            th0 = new Thread(t0);
            th0.IsBackground = true;
            th0.Start();
            for (int i = 0; i < 10; i++)
                Console.WriteLine($"Fore thread: {i}");

            Console.WriteLine("Foreground thread ended");
        }

        static void SendDataToSecondaryThread()
        {
            // Подготовка к запуску вторичного потока предполагает создание объекта класса потока. 
            // В этот момент вся необходимая для работы потока информация передается через параметры конструктора.
            // Здесь переданы необходимые детали, которые будут составлены стандартным образом в строку методом WriteLine. 
            WorkThread tws = new WorkThread("Отображаем значение, переданное потоку {0}.", 1234567890);
            Thread t = new Thread(new ThreadStart(tws.ThreadProc)); // Создаём и запускаем объект потока
            t.Start();
            Console.WriteLine("Первичный поток поработал. Теперь ждет...");
            t.Join();
            Console.WriteLine("Сообщение из первичного потока: Вторичный поток отработал.");
            Console.WriteLine("Сообщение из первичного потока: Главный поток остановлен. ");
        }

        static void CallBackExample()
        {
            // Supply the state information required by the task.
            WorkThreadCallBack tws = new WorkThreadCallBack("Отображаем значение, переданное потоку {0}.", 1234567890, new CallbackMethod(ResultCallback));
            Thread t = new Thread(new ThreadStart(tws.ThreadProc));
            t.Start();
            Console.WriteLine("Первичный поток поработал. Теперь ждет...");
            t.Join();
            Console.WriteLine("Вторичный поток отработал. Главный поток остановлен.");
        }

        // Callback-метод, естественно, соответствует сигнатуре callback класса делегата.
        public static void ResultCallback(int callResult)
        {
            Console.WriteLine($"Вторичный поток вернул результат: {result=callResult}");
        }
    }

    class Worker
    {
        int allTimes;
        public Worker(int tKey)
        {
            allTimes = tKey;
        }
        
        public void DoItEasy() // Тело рабочей функции...
        {
            for (int i = 0; i < allTimes; i++)
                Console.WriteLine($"Back thread >>>> {i}\r");

            Console.WriteLine("\nBackground thread was here!");
        }
    }

    public class WorkThread
    {
        private string entryInformation;
        private int value;

        // Конструктор получает всю необходимую информацию через параметры.
        public WorkThread(string text, int number)
        {
            entryInformation = text;
            value = number;
        }

        // Рабочий метод потока непосредственно после своего запуска
        // получает доступ ко всей необходимой ранее подготовленной информации.
        public void ThreadProc()
        {
            Console.WriteLine(entryInformation, value);
        }
    }



    // Класс-делегат задает сигнатуру callback-методу.
    public delegate void CallbackMethod(int lineCount);

    // Класс WorkThreadCallBack включает необходимую информацию,
    // метод и делегат для вызова метода, который запускается после выполнения задачи.
    public class WorkThreadCallBack
    {   // Входная информация.
        private string entryInformation;
        private int value;
        // Ссылка на объект - представитель класса-делегата, с помощью которого
        // вызывается метод обратного вызова. Сам класс-делегат объявляется позже.
        private CallbackMethod callback;

        // Конструктор получает входную информацию и настраивает callback delegate.
        public WorkThreadCallBack(string text, int number, CallbackMethod callbackDelegate)
        {
            entryInformation = text;
            value = number;
            callback = callbackDelegate;
        }

        // Метод, обеспечивающий выполнение поставленной задачи:
        // составляет строку и после дополнительной проверки настройки callback-делегата обеспечивает вызов метода.
        public void ThreadProc()
        {
            int localResult = 123;
            Console.WriteLine(entryInformation, value);
            if (callback != null)
                callback(localResult); // Вызвал ЧУЖОЙ МЕТОД в СВОЕМ потоке.
        }
    }
}