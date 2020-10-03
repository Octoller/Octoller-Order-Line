# Octoller.LineCommander
> Небольшая библиотека написанная (и находящаяся в доработке) в качестве проекта для обучения.

Библиотека позволяет, реализуя специальные интерфейсы, создавать обработчики строковых команд, перекладывая парсинг строки и запуск обработчиков на внутренние классы. 

### На текущий момент поддерживаются следующие возможности:
* Передаяа любого колличества аргументов в команду посредствам разделения символом `,`

      `CL> команда:аргумент1,аргумент2,аргумент3... аргументN`
      

* Выполнение цепочки команд с использованием `команд-переходов`, определяющих условие перехода к следующей команде. 
 
      `CL> команда:аргумент,аргумент,аргумент & команда:аргумент && команда:аргумент,аргумент || команда`  
       
  **&**  — используется для разделения нескольких команд в одной командной строке. Команды выполняются последовательно.

  **&&** — запускает команду, стоящую за символом &&, только если команда, стоящая перед этим символом была выполнена успешно.

  **||** — запускает команду, стоящую за символом ||, только если команда, стоящая перед символом || не была выполнена.
  
  
* Пропуск через команды объекта реализующего интерфейс **IChContext**
