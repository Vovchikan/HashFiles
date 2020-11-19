

## HashFiles

Консольная программа, которая для заданных каталогов, файлов, или подкатологов (если включен флаг ```recursive```) рассчитывает хэш-суммы содержащихся в них файлов и сохраняет результаты расчетов.

### Механизм расчетов:

#### Работа программы разбита на несколько потоков: 
- первый поток обходит каталоги, ищет  файлы, для которых нужно выполнить расчет, и помещает их в очередь. 
- **Рабочие потоки** извлекают из очереди описания файлов и выполняют для их содержимого расчет хэш-суммы. 
- Еще один поток сохраняет результаты вычисления.

#### Сохранение результатов:
- Сохраняет результат расчета по каждому файлу.
- Сохраняет информацию об ошибках, произошедших в процессе обработки файлов.

### Пояснения к реализации

- Язык реализации: C#;
- СУБД: MSSQL;
- Синхронизация потоков выполнятся с помощью базовых механизмов платформы (без использования TPL, Concurrent-коллекций);
- Для расчета хэш-сумм используются классы платформы .NET;

---

### Использование параметров командной строки

Для получения списка всех возможных команд - ```./HashFiles.exe help``` или ```./HashFiles.exe --help```
#### Вывод:
```
HashFiles 0.1.7627.39868
Copyright (c) 2020 https://github.com/Vovchikan

  console    Count hash sum of files and print results.
  file       Count hash sum of files and write results in file.
  bd         Count hash sum of files and add results to bd.
  help       Display more information on a specific command.
  version    Display version information.
```
Для получения всех возможных опций ```команды``` - ```./HashFiles.exe команда help``` или ```./HashFiles.exe команда --help```
#### Пример:
```
$ ./HashFiles.exe file --help
HashFiles 0.1.7627.39868
Copyright (c) 2020 https://github.com/Vovchikan

  o, output     (Default: .\data) Path to directory, where output files will be
                created.
  n, name       (Default: output.txt) Name of output file. For ex. -
                "output.txt".
  overwrite     (Default: false) Overwrite the output file, if this file already
                exists.
  p, paths      Required. Paths to files\dirs for hash sums calculation.
  r             (Default: false) Turn on recursive calculation from directories.
  threads       (Default: 2) Count of threads for calculation hash sum.
  v, verbose    (Default: false)
  help          Display more information on a specific command.
  version       Display version information.

  ```



