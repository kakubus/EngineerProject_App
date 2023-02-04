# __RoboApp__

<p align="left"><i>Autor: Jakub Kaniowski</i></p>

<hr/>
<h5>Politechnika Śląska</br>
Wydział Automatyki Elektroniki i Informatyki</br>
Kierunek: Informatyka. Specjalność: Oprogramowanie systemowe</h5>


<hr/>
<p align="center"><i> System sterowania robotem mobilnym </i>- <b>aplikacja sterująca</b> </p>
<hr/>

> Projekt realizowany w ramach zaliczenia zajęć: Platforma .NET. Aplikacja wieloplatformowa (.NET MAUI) umożliwiająca sterowanie robotem mobilnym typu Mecanum, wykonanego w ramach [pracy inżynierskiej](https://github.com/kakubus/EngineerProject_View/) (2022)


## Opis projektu

Projekt zakłada stworzenie aplikacji wieloplatformowej (z naciskiem na dostępność na systemie Windows oraz Android, iOS – brak konta dewelopera), umożliwiającej sterowanie robotem mobilnym z kołami szwedzkimi, stworzonego w ramach pracy inżynierskiej.

Robot mobilny posiada sterownik oparty na mikrokontrolerze **__ESP-32-WROOM__**, co sprawia, że możliwa jest bezpośrednia bezprzewodowa komunikacja robota z komputerem. Robot pełni funkcję punktu dostępowego, a urządzenie sterujące (komputer, tablet, telefon) pełni funkcję klienta.

Komunikacja jest realizowana przez sieć bezprzewodową 2.4GHz w standardzie 802.11b/g/n. Do wymiany informacji wykorzystano protokół TCP. Porty docelowe są następujące:

*	Robot przyjmuje komunikaty na porcie **1000**. 
*	Robot wysyła komunikaty do portu **60890** na urządzeniu klienckim.

Utworzona sieć pomiędzy hostami skonfigurowana jest w następujący sposób:

### Konfiguracja sieci bezprzewodowej

| Właściwość | Wartość |
| :---         |     :---:      |
| SSID  | ROBO-1     | 
| Hasło   | Robot1234     |
| Brama domyślna*   | 192.168.0.1     |
| Maska podsieci   | 255.255.255.252    |
| Pula adresów   | 192.168.0.1-192.168.0.2    |

> *funkcję pełni robot

Stworzona podsieć oferuje jeden adres IP, możliwy do przypisania urządzeniu: 192.168.0.2

Każde inne “nadmiarowe” urządzenie podłączone do punktu dostępowego zyska inny adres IP, nienależący do docelowej podsieci, wykorzystywanej do komunikacji. Dzieki takiej konfiguracji, nie ma potrzeby rozróżniania skąd nadchodzi komunikat. Rozwiązanie jest proste i należy pamiętać o zasadzie “Kto pierwszy, ten lepszy” - tj. pierwszy host przyłączony do sieci zyska możliwość komunikacji. 

### Diagram komunikacji
<p align="center">
<img width="600" height="150" src="https://user-images.githubusercontent.com/73018121/216727834-0bc6e31a-bfa5-4d01-a6bf-6801a95279e3.png">
</p>

## Opis aplikacji
Do stworzenia aplikacji wykorzystano środowisko Visual Studio 2022. Aplikacja została napisana w języku C# z wykorzystaniem środowiska uruchomieniowego .NET MAUI.

Aplikacja wykorzystuje podejście asynchroniczne wykonywania programu, co gwarantuje responsywność aplikacji na szybkie i stałe nadzorowanie transmisji. Ponadto w celach przyszłościowej analizy danych będzie wykorzystana technologia LINQ. 

Obecnie jest ona mało wykorzystywana z uwagi na początkowe stadium rozwoju aplikacji i oprogramowania sterownika robota. Do komunikacji aplikacji z robotem służy wbudowana biblioteka umożliwiająca asynchroniczną komunikację z wykorzystaniem protokołu połączeniowego TCP.

### Wymagania aplikacji

Tworzona aplikacja powinna umożliwiać sterowanie robotem mobilnym w czasie rzeczywistym. Poprzez sterowanie rozumie się:

*	Nadawanie kierunku poruszania
*	Ustalanie prędkości ruchu
*	Uwzględnianie wysterowania napędów robota, tak aby wykonał on określony ruch (np. Jazda do przodu, lub w bok)
*	W przypadku nieprzewidywanego zachowania się robota, możliwości próby zdalnego zatrzymania awaryjnego i zerwania połączenia (tzw. Wprowadzenie w tryb oczekiwania)

Ponadto aplikacja powinna umożliwiać odczytanie komunikatów nadawanych przez sterownik, takich jak:
*	Prędkość obrotowa poszczególnych napędów
*	Kierunek obrotu poszczególnych z kół
*	Inne parametry przesyłane w komunikacie.


***

## Widoki aplikacji - Interfejs

Aplikacja została stworzona z wykorzystaniem technologii .NET MAUI. Wykorzystano gotowe elementy służące do obsługi interfejsu. Całość prezentuje się następująco:


<p align="center">
<img width="230" height="400" src="https://user-images.githubusercontent.com/73018121/216775430-202190ff-7de3-4f59-b99e-a726c67663d2.png">
<img width="230" height="400" src="https://user-images.githubusercontent.com/73018121/216775440-23898c4e-3ca8-49bd-98c5-96052a19d6b9.png">
<img width="230" height="400" src="https://user-images.githubusercontent.com/73018121/216775474-e8b5db16-bdf0-4e4f-81ad-48b2649cd388.png">
<img width="230" height="400" src="https://user-images.githubusercontent.com/73018121/216775501-ceeaec4e-c31c-4819-94c0-fbf822c9a85c.png">
</p>

#### Widoki aplikacji. 
* Pierwszy po lewej - aplikacja przed połączeniem się z robotem. 
* Drugi w kolejności – aplikacja odbierająca komunikaty
* Trzeci w kolejności -  aplikacja po wciśnięciu jednego z przycisków nadających kierunek ruchu. 
* Ostatni -  aplikacja po wciśnięciu przycisku “bezpieczeństwa”.)

***
