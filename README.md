---Tema 1---
Aplicatie pentru gestionarea comenzilor unei patiserii

Operatii care vor fi implementate in cadrul aplicatie:
	- Adaugare comanda
	- Stergere comanda
	- Modificare comanda
	- Vizualizare comenzi
	- Cautare comanda
	- Adaugare produs
	- Stergere produs
	- Modificare produs
	- Vizualizare produse (Meniul)
	- Cautare comanda
Pentru fiecare comanda se vor stoca urmatoarele informatii:
	- ID comanda 
	- Nume si prenume client
	- Numar de telefon
	- Produs comandat
	- Detaliile produsului (daca este necesar de ex.:arome, culori, decoratiuni, alergii etc.)
	- Cantitate
	- Pret unitar
	- Pret total (calculat automat de catre program ca fiind cantitate*pret unitar)
	- Data livrarii
	- Status comenzii (in asteptare, in procesare, finalizata)
	- Status plata (neplatita, platita)

---Tema 2---
Clasele necesare implementarii aplicatiei:
	- Clasa Client: va contine nume, prenume si numar de telefon al clientului (NU ESTE NECESARA DEOARECE COMPLICA IMPLEMENTAREA SI NU BENEFICIAZA APLICATIA INTR-UN MOD SEMNIFICATIV)
	- Clasa Produs: va contine numele produsului, ID-ul, detaliile acestuia si pretul unitar
	- Clasa Comanda: va contine _un obiect de tip Client_, o lista de produse, ID, cantitatea comandata, pretul total, data livrarii, statusul comenzii si statusul platii
	- Clasa ArticolComanda: va contine un obiect de tip Produs si cantitatea comandata pentru acel produs
	- Clasa Manager: "creierul" aplicatiei, gestioneaza listele de obiecte si operatiile de adaugare, stergere, etc.

Pentru creearea unei comenzi avem 2 optiuni:
	- Creearea unei comenzi manual: se va introduce manual toate informatiile necesare pentru o comanda noua 
			- dezavantaj: se poate introduce orice produs si date eronate 
			- avantaj: e mai simplu de implementat
	- Creearea unei comenzi dintr-un meniu: se va selecta fiecare produs si cantitatea sa dupa care se pot introduce restul informatiilor
			- avantaj: elimina posibilitatea introducerii unor produse inexistente sau date eronate legate de produse
			- dezavantaj: creearea meniului poate fi mai complexa si limiteaza numarul de produse disponibile

Voi implementa a doua optiune deoarece:
	- Este mai usor de navigat pentru utilizator
	- Problema limitarii numarului de produse poate fi eliminata prin creearea posibilitatii de adaugare a unor produse noi in meniu
	- Elimina posibilitatea introducerii unor produse inexistente sau date eronate legate de produse

Pentru partea de creeare a comenzii pot folosi un meniu al produselor cu ID-uri iar utilizatorul introduce ID-ul produsului dorit si cantitatea
Totusi nu e necesara creearea clasei Client deoarece informatiile despre client pot fi stocate direct in clasa Comanda, in deosebire de produse care vor fi folosite in mai multe comenzi mult mai frecvent, multi clienti pot fi unici
Pentru statusul comenzii si statusul platii pot fi folosite enumerari pentru a limita valorile posibile si a face codul mai usor de inteles
Pentru cantitatea de produs comandata voi mai introduce o clasa care face direct legatura intre produs si cantitate, nu voi folosi 2 vectori, pot aparea probleme la partea de stergere sau adaugare cu sincronizarea celor 2 vectori

1. Clasa Produs
	Parametrii:
		- ID -int
		- Nume -string
		- PretUnitar -decimal
		- Detalii -string
	Metode:
		- Constructor
		- Getteri si Setteri

2. Clasa ArticolComanda
	Parametrii:
		- ProdusComandat -Produs
		- Cantitate -int
		- PretTotalArticol -decimal (calculeaza automat ca fiind Cantitate*PretUnitar)
	Metode:
		- Constructor
		- Getteri si Setteri

3. Clasa Comanda
	Parametrii:
		- ID -int
		- NumeClient -string
		- PrenumeClient -string
		- NumarTelefon -string
		- Produse -Lista de obiecte tip ArticolComanda
		- PretTotal -decimal (calculat automat ca fiind suma preturilor totale ale articolelor din comanda)
		- DataLivrarii -DateTime
		- StatusComanda -enum
		- StatusPlata -enum 
	Metode:
		- Constructor
		- Getteri si Setteri
		- AdaugaProdus

4. Clasa Manager
	Parametrii:
		- _comenzi -Lista de obiecte tip Comanda
		- _meniu -List de obiecte tip Produs
		- _urmatorulIDComanda -int
		- _urmatorulIDProdus -int
	Metode:
		- Constructor
		- AdaugaProdus
		- ObtineMeniu
		- CautaProdus
		- ModificaProdus
		- StergeProdus
		- AdaugaComanda
		- ObtineComenzi
		- CautaComanda
		- ModificaComanda
		- StergeComanda