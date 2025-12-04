BookApi - Backend Guide (Pedagogisk)

Vad är det här?

Tänk dig ett bibliotek där varje person har sitt eget hyllsystem. Ingen annan kan se eller röra dina böcker. Det är exakt vad vår backend gör!

Enkelt förklarat:
- Du skapar ett konto (som ett bibliotekskort)
- Du får en speciell nyckel (JWT token)
- Med nyckeln kan du bara öppna DINA hyllor
- Ingen annan kan se dina böcker eller citat



Systemets Delar


┌─────────────────────────────────────────┐
│        BOOKAPI BACKEND                 │
├─────────────────────────────────────────┤
│                                         │
│  1. SÄKERHET (JWT)                   │
│     ↓ Kontrollerar att du är du        │
│                                         │
│  2. ANVÄNDARE                        │
│     ↓ Registrering och Inloggning      │
│                                         │
│  3. BÖCKER                           │
│     ↓ Skapa, Läsa, Uppdatera, Ta bort  │
│                                         │
│  4. CITAT                            │
│     ↓ Spara citat från böcker          │
│                                         │
│  5. DATABAS                         │
│     ↓ Lagrar all data                  │
│                                         │
└─────────────────────────────────────────┘


DEL 1: JWT Authentication (Säkerhetsnyckeln)

Vad är JWT?

Tänk på JWT som en magisk nyckel med ditt namn inristat. När du loggar in får du denna nyckel, och varje gång du frågar efter något visar du upp nyckeln.


┌───────────────────────────────────┐
│  JWT TOKEN (Din magiska nyckel)   │
├───────────────────────────────────┤
│   Namn: "Johan"                 │
│   Email: "johan@mail.com"       │
│   ID: 42                        │
│   Giltig till: 2024-12-02 14:00 │
│   Signatur: "xxxxx..."          │
└───────────────────────────────────┘


Hur fungerar det?

Steg 1: Konfiguration

Vi säger åt systemet hur det ska skapa nycklar:

SecretKey: En hemlig kod som endast backend känner till (som lösenordet till ett kassaskåp)
Issuer: Vem som ger ut nyckeln (vår backend)
Audience: Vem som får använda nyckeln (våra användare)
Giltighetstid: 24 timmar (nyckeln slutar fungera efter en dag)


Inställningar:
┌────────────────────────────┐
│ SecretKey: "xyz123..."     │ ← Hemlig kod
│ Issuer: "BookApi"          │ ← Vi utfärdar nyckeln
│ Audience: "BookApiUsers"   │ ← För våra användare
│ Giltig: 24 timmar          │ ← Går ut efter en dag
└────────────────────────────┘


Steg 2: När någon försöker använda systemet


Användare skickar nyckel → Backend kollar:
                           ├─ Är signaturen äkta? ✓
                           ├─ Kommer från rätt ställe? ✓
                           ├─ Är den inte utgången? ✓
                           └─ OK! Användaren får komma in ✓


Steg 3: Middleware-ordning (Viktigt!)

Tänk på det som säkerhetskontroller på en flygplats - ordningen är viktig:

1. CORS Check       ← "Får du ens prata med oss?" (olika webbplatser)
   ↓
2. Authentication   ← "Vem är du?" (läs nyckeln)
   ↓
3. Authorization    ← "Får du göra det här?" (kolla rättigheter)
   ↓
4. Controller       ← "Gör vad du ska" (hämta böcker osv)


Om vi ändrar ordningen blir det kaos! Som att kolla bagage innan man kollar biljetten.

Steg 4: Skapa en nyckel

När du loggar in skapar vi en nyckel med din info:


Din info → Backend stoppar in det i nyckeln → Du får nyckeln

Exempel nyckel:
"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0MiIsIm5hbWUiOiJKb2hhbiJ9.xxxxx"

Den innehåller (kodat):
- Ditt ID
- Ditt användarnamn
- Din email
- När den går ut


DEL 2: Användare (Registrering & Inloggning)

Vad händer när du skapar ett konto?


TYP DIN INFO                BACKEND GÖR               DU FÅR TILLBAKA
┌──────────────┐           ┌──────────────┐          ┌──────────────┐
│ Användarnamn │           │ Kollar om    │          │ Din magiska  │
│ Email        │ ────────→ │ email finns  │ ───────→ │ nyckel (JWT) │
│ Lösenord     │           │              │          │              │
└──────────────┘           │ Hashar       │          │ + Din info   │
                           │ lösenord     │          └──────────────┘
                           │              │
                           │ Sparar user  │
                           │              │
                           │ Skapar token │
                           └──────────────┘
```

Registrering - Steg för steg

Steg 1: Du fyller i formulär

📝 Registreringsformulär:
┌─────────────────────────┐
│ Användarnamn: johan     │
│ Email: johan@mail.com   │
│ Lösenord: hemligt123    │
│                         │
│     [Registrera] 🚀     │
└─────────────────────────┘


Steg 2: Backend kollar om email finns redan

Backend tittar i databasen:
Finns johan@mail.com redan?"

Om JA  → "Email redan använd"
Om NEJ → ✓ Fortsätt


Steg 3: Lösenordet hashas (görs om till kod)**

Ditt lösenord: "hemligt123"
        ↓
   [HASHER] 
        ↓
Sparas som: "x7k9m2p5...

Varför? Om någon hackar databasen ser de bara "x7k9m2p5...
och inte "hemligt123"!


Steg 4: Användaren sparas

Databas får ny rad:
┌────┬─────────┬─────────────────┬──────────────┐
│ ID │  Namn   │     Email       │  Lösenord    │
├────┼─────────┼─────────────────┼──────────────┤
│ 42 │ johan   │ johan@mail.com  │ x7k9m2p5...  │
└────┴─────────┴─────────────────┴──────────────┘
```

Steg 5: Token skapas och skickas till dig

Backend skapar nyckel:
┌─────────────────────────┐
│ JWT Token               │
│ Innehåller:             │
│ - ID: 42                │
│ - Namn: johan           │
│ - Email: johan@mail.com │
│ - Giltig till: imorgon  │
└─────────────────────────┘
```

Steg 6: Du får tillbaka token
Registrering klar!

{
  "token": "eyJhbG...",
  "username": "johan",
  "email": "johan@mail.com"
}

Nu sparar din webbläsare token i localStorage
(som att lägga nyckeln i fickan)


Inloggning - Vad händer?

1. Du skriver email + lösenord
   ↓
2. Backend hittar dig i databasen
   ↓
3. Kollar om lösenordet stämmer (hashar det du skrev och jämför)
   ↓
4. Om rätt → Skapa ny token
   ↓
5. Ge dig token (du får en ny nyckel)


Exempel:

Logga in:
Email: johan@mail.com
Lösenord: hemligt123
         ↓
Backend: "Hashar hemligt123..."
         Blir: "x7k9m2p5..."
         "Jämför med sparat: x7k9m2p5..."
         "MATCH! ✓"
         ↓
         Skapar ny token
         ↓
Du får: { token: "eyJ...", username: "johan", ... }


DEL 3: Böcker (CRUD Operations)

CRUD = **C**reate (Skapa), **R**ead (Läsa), **U**pdate (Uppdatera), **D**elete (Ta bort)

Hur systemet vet vad som är DITT

Varje bok har ett "ägarID":

Böcker i systemet:
┌────┬──────────────────┬───────────────┬──────────┐
│ ID │      Titel       │    Författare │  ÄgarID  │
├────┼──────────────────┼───────────────┼──────────┤
│ 1  │ Harry Potter     │ J.K. Rowling  │   42     │ ← Johan's
│ 2  │ Lord of Rings    │ Tolkien       │   42     │ ← Johan's
│ 3  │ 1984             │ Orwell        │   55     │ ← Anna's
│ 4  │ Hobbit           │ Tolkien       │   55     │ ← Anna's
└────┴──────────────────┴───────────────┴──────────┘

När Johan (ID: 42) frågar efter böcker:
Backend: "Visa bara böcker där ÄgarID = 42"
Johan ser: bok 1 och 2 (inte 3 och 4!)


1. Hämta Alla Böcker (GET /api/books)

Vad händer:

1. Du skickar din nyckel (token)
   ↓
2. Backend läser nyckeln: "Aha, det är Johan (ID: 42)"
   ↓
3. Backend går till databasen:
   "Ge mig alla böcker där ÄgarID = 42"
   ↓
4. Du får bara dina böcker tillbaka


Visuellt:

DIN NYCKEL           BACKEND                RESULTAT
┌──────────┐        ┌──────────┐           ┌──────────┐
│ Token    │───────→│ Läser ID │──────────→│ Dina 2   │
│ ID: 42   │        │ Filtrerar│           │ böcker   │
└──────────┘        │ på ID:42 │           └──────────┘
                    └──────────┘


Kod gör:
csharp
1. Läs vem användaren är från token
2. Fråga databasen: "Ge böcker med detta ID"
3. Skicka tillbaka listan
```

2. Hämta En Bok (GET /api/books/5)

Extra säkerhet här!


Du frågar efter bok ID 5
   ↓
Backend kollar:
   "Finns bok 5?" → JA
   "Äger DU bok 5?" → Kollar ägarID
   
Om du äger den → ✓ Här är boken
Om du INTE äger den → "404 Not Found"


Varför säga Not Found" istället för "Du äger inte denna

Tänk så här: Om någon frågar "Finns pengar i kassaskåp 5?" 
Säger vi inte "Ja men inte dina" - vi säger "Vilket kassaskåp?" 
På så sätt avslöjar vi inget!


Johan frågar efter bok ID 3
   ↓
Backend ser: Bok 3 ägs av Anna (ID: 55)
              Johan är ID: 42
              55 ≠ 42
   ↓
Svarar: "404 Not Found" (som att boken inte finns)


3. Skapa Bok (POST /api/books)

Flöde


1. Du fyller i formulär:
   ┌─────────────────────────┐
   │ Titel: "Harry Potter"   │
   │ Författare: "Rowling"   │
   │ Genre: "Fantasy"        │
   │ År: 1997                │
   │                         │
   │    [Spara]              │
   └─────────────────────────┘

2. Backend tar din data + läser ditt ID från token
   
3. Skapar ny bok:
   ┌────┬──────────────┬──────────┬─────────┬──────────┐
   │ ID │    Titel     │ Författare│  Genre  │  ÄgarID  │
   ├────┼──────────────┼──────────┼─────────┼──────────┤
   │ 6  │Harry Potter  │ Rowling  │ Fantasy │   42     │ ← NYTT!
   └────┴──────────────┴──────────┴─────────┴──────────┘
                                              ↑
                                         Automatiskt satt till ditt ID!

4. Du får tillbaka boken med ID 6

Viktigt: Backend sätter ALLTID ägarID till DIG. Du kan inte skapa böcker för någon annan!

4. Uppdatera Bok (PUT /api/books/6)

Partial Update** = Du behöver bara skicka det som ska ändras


Du vill ändra titel på bok 6:
┌──────────────────────────┐
│ Titel: "HP och de vises │
│         sten"            │
│                          │
│   [Uppdatera]            │
└──────────────────────────┘

Backend:
1. "Finns bok 6?" → ✓
2. "Äger du bok 6?" → Kollar (ID: 42 = 42) → ✓
3. "Uppdatera bara Titel (resten samma)"

Före:
┌────┬──────────────┬──────────┐
│ 6  │Harry Potter  │ Rowling  │
└────┴──────────────┴──────────┘

Efter:
┌────┬──────────────────────┬──────────┐
│ 6  │HP och de vises sten  │ Rowling  │ ← Endast titel ändrad!
└────┴──────────────────────┴──────────┘


Om du försöker ändra någon annans bok:

Du (ID: 42) försöker ändra bok 3 (ägs av ID: 55)
   ↓
Backend: "Äger du bok 3?"
         42 ≠ 55
         NEJ!
   ↓
Resultat: "404 Not Found"


5. Ta Bort Bok (DELETE /api/books/6)


Du klickar "Ta bort" på bok 6
   ↓
Backend:
1. "Finns bok 6?" → ✓
2. "Äger du bok 6?" → ✓
3. "OK, tar bort..."
   ↓
Bok 6 raderas från databasen
   ↓
 "204 No Content" (success!)


Säkerhet:

Scenario: Du försöker ta bort bok 3 (Anna's)

Backend: "Äger du bok 3?" → NEJ
         ↓
         "404 Not Found"
         
Bok 3 finns kvar (phu!)




 DEL 4: Citat från Böcker

Vad är speciellt med citat?

Viktig detalj: Citatet har sin EGEN författare-fält!

Bok:
┌──────────────────────────┐
│ Titel: "Antologi"        │
│ Författare: "Flera"      │
└──────────────────────────┘
          ↓
     Innehåller
          ↓
          
   Citat 1:                Citat 2:
┌────────────────────┐    ┌────────────────────┐
│ Text: "Att vara..." │    │ Text: "Livet är..." │
│ Författare: "Strindberg"│ │ Författare: "Lagerlöf"│
└────────────────────┘    └────────────────────┘
    ↑                          ↑
   INTE samma som bokens     INTE samma som bokens
   författare!                författare!


Varför För att du kanske har en antologi med många författare, eller vill ange vem som sa citatet även om det står i någon annans bok.

Struktur av ett citat


┌─────────────────────────────────┐
│ ID: 1                           │
│ Text: "So we beat on..."        │
│ Författare: "F. Scott Fitzgerald"│ ← Kan vara vad som helst
│ Sidnummer: 180                  │
│ Skapad: 2024-12-01 10:30       │
│ BokID: 5                        │ ← Från vilken bok
│ ÄgarID: 42                      │ ← Vem som sparat det
└─────────────────────────────────┘


Relationer


ANVÄNDARE (Johan, ID: 42)
    │
    ├─── Äger BOK 1 (Harry Potter)
    │         │
    │         └─── Innehåller CITAT 1 ("Happiness can be found...")
    │              
    ├─── Äger BOK 2 (Lord of Rings)
    │         │
    │         ├─── Innehåller CITAT 2 ("All we have to decide...")
    │         └─── Innehåller CITAT 3 ("Not all those who wander...")
    │
    └─── Äger CITAT 4 (från någon annans bok, men Johan sparade det)

1. Hämta Alla Citat (GET /api/quotes)


Du frågar: "Visa mina citat"
   ↓
Backend läser din nyckel: "Johan (ID: 42)"
   ↓
Går till databasen:
   "Ge alla citat där ÄgarID = 42"
   ↓
Plus, lägg till bokens titel också (JOIN)
   ↓
Du får lista:
[
  { id: 1, text: "...", från_bok: "Harry Potter" },
  { id: 2, text: "...", från_bok: "LOTR" },
  { id: 3, text: "...", från_bok: "LOTR" }
]


2. Hämta Citat från En Specifik Bok (GET /api/quotes/book/5)

Användningsfall: Du tittar på bok 5 och vill se alla citat du sparat från den.


Du: "Visa citat från bok 5"
   ↓
Backend:
   "Ge citat där BokID = 5 OCH ÄgarID = 42"
   ↓
Resultat: Bara DINA citat från just den boken


Visuellt:
 Bok 5: "The Great Gatsby"
    │
    ├─  Citat 1 (Johan's) ✓ Visas
    ├─  Citat 2 (Johan's) ✓ Visas
    ├─  Citat 3 (Anna's)  ✗ Visas INTE (annan ägare)
    └─  Citat 4 (Johan's) ✓ Visas


3. Skapa Citat (POST /api/quotes)

Scenario:Du läser sida 180 i "The Great Gatsby" och hittar ett bra citat.


1. Du fyller i:
   ┌────────────────────────────────┐
   │ Text: "So we beat on..."      │
   │ Författare: "F. Scott Fitzgerald" │
   │ Sidnummer: 180                 │
   │ Från bok: 5 (The Great Gatsby) │
   │                                │
   │        [Spara]                 │
   └────────────────────────────────┘

2. Backend kollar: "Finns bok 5?" → ✓

3. Skapar citat:
   ┌────┬─────────────────┬──────────────┬────┬───────┬────────┐
   │ ID │      Text       │  Författare  │Sid │ BokID │ ÄgarID │
   ├────┼─────────────────┼──────────────┼────┼───────┼────────┤
   │ 10 │ So we beat on...│ Fitzgerald   │180 │   5   │   42   │
   └────┴─────────────────┴──────────────┴────┴───────┴────────┘
                                                  ↑       ↑
                                               Från    Automatiskt
                                               bok 5   ditt ID!


4. Uppdatera Citat (PUT /api/quotes/10)

Special: Du kan ändra författaren oberoende av boken!


Exempel: Du märker att citatet var från en annan person i boken

Före:
┌────┬─────────────────┬──────────────┐
│ 10 │ So we beat on...│ Fitzgerald   │
└────┴─────────────────┴──────────────┘

Du ändrar:
┌─────────────────────────────┐
│ Författare: "Nick Carraway" │ ← Berättaren, inte bokförfattaren
│                             │
│      [Uppdatera]            │
└─────────────────────────────┘

Efter:
┌────┬─────────────────┬──────────────┐
│ 10 │ So we beat on...│ Nick Carraway│ ← Ändrat
└────┴─────────────────┴──────────────┘


Säkerhet: Samma som böcker - du kan bara uppdatera DINA citat.

5. Ta Bort Citat (DELETE /api/quotes/10)


Du: "Ta bort citat 10"
   ↓
Backend:
   "Finns citat 10?" → ✓
   "Äger du citat 10?" → ✓ (ID: 42 = 42)
   "OK, tar bort..."
   ↓
Raderat!


DEL 5: Databasen

Vad är en databas?

Tänk på det som ett GIGANTISKT Excel-ark med flera flikar (tabeller).

DATABASE: "BookApiDb"
├─ Tabell: Users
│  ┌────┬──────────┬─────────────────┬──────────────┐
│  │ ID │   Namn   │      Email      │   Lösenord   │
│  ├────┼──────────┼─────────────────┼──────────────┤
│  │ 42 │  johan   │ johan@mail.com  │ x7k9m2p5...  │
│  │ 55 │  anna    │ anna@mail.com   │ y8n3k1m4...  │
│  └────┴──────────┴─────────────────┴──────────────┘
│
├─ Tabell: Books
│  ┌────┬──────────────────┬───────────┬──────────┐
│  │ ID │      Titel       │ Författare│  ÄgarID  │
│  ├────┼──────────────────┼───────────┼──────────┤
│  │ 1  │ Harry Potter     │ Rowling   │    42    │
│  │ 2  │ Lord of Rings    │ Tolkien   │    42    │
│  │ 3  │ 1984             │ Orwell    │    55    │
│  └────┴──────────────────┴───────────┴──────────┘
│
└─ Tabell: Quotes
   ┌────┬─────────────────┬────────────┬───────┬────────┐
   │ ID │      Text       │  Författare│ BokID │ ÄgarID │
   ├────┼─────────────────┼────────────┼───────┼────────┤
   │ 1  │ Happiness...    │ Rowling    │   1   │   42   │
   │ 2  │ All we have...  │ Tolkien    │   2   │   42   │
   │ 3  │ War is peace... │ Orwell     │   3   │   55   │
   └────┴─────────────────┴────────────┴───────┴────────┘

Relationer mellan tabeller

USERS Tabell
    │
    │ (en användare kan ha många böcker)
    ↓
BOOKS Tabell
    │
    │ (en bok kan ha många citat)
    ↓
QUOTES Tabell


Visuellt med pilar:

User 42 (Johan)
    │
    ├──→ Book 1
    │      └──→ Quote 1
    │
    └──→ Book 2
           ├──→ Quote 2
           └──→ Quote 3

User 55 (Anna)
    │
    └──→ Book 3
           └──→ Quote 4


InMemory Database (Nuvarande)

Vad betyder "InMemory"?

Tänk på det som att skriva i sand på stranden:
- Super snabbt att skriva och läsa
- Inget krångel med installation
- När vågen kommer (servern startas om) försvinner allt


Server startar → Databas tom
Du lägger till data → Lagras i minnet (RAM)
Server stängs av → POFF! All data borta
Server startar igen → Databas tom igen


Bra för: Utveckling och testning  
Dåligt för: Produktion (riktiga användare)


DEL 6: Säkerhetslager

Tänk på säkerheten som lager i en lök - många lager skyddar bättre!


                      Internet
                         ↓
        ╔════════════════════════════════╗
        ║  Lager 1: CORS                ║
        ║  "Får din webbplats prata     ║
        ║   med oss?"                   ║
        ╚════════════════════════════════╝
                         ↓
        ╔════════════════════════════════╗
        ║  Lager 2: Token Validation    ║
        ║  "Är nyckeln äkta?"           ║
        ║  "Är den inte utgången?"      ║
        ╚════════════════════════════════╝
                         ↓
        ╔════════════════════════════════╗
        ║  Lager 3: [Authorize]         ║
        ║  "Har du nyckel över huvud     ║
        ║   taget?"                     ║
        ╚════════════════════════════════╝
                         ↓
        ╔════════════════════════════════╗
        ║  Lager 4: GetUserId()         ║
        ║  "Vem är du?"                 ║
        ║  (Läser ID från nyckel)       ║
        ╚════════════════════════════════╝
                         ↓
        ╔════════════════════════════════╗
        ║  Lager 5: Ownership Check     ║
        ║  "Äger DU den här boken?"     ║
        ║  (Kollar ÄgarID)              ║
        ╚════════════════════════════════╝
                         ↓
                  OK! Visa data


Lager 1: CORS (Cross-Origin Resource Sharing)

Problem: Din webbläsare vill skydda dig.


Din app: http://localhost:4200
Backend:  http://localhost:5057

Browser tänker: "Två olika adresser!
Browser blockerar: 

Lösning: Vi säger åt backend "Det är OK, låt localhost:4200 prata med mig

Backend säger: "Jag tillåter:
- http://localhost:4200 ✓
- http://localhost:4201 ✓
- http://localhost:63033 ✓

Andra: 

Lager 2: Token Validation

Backend kollar fyra saker på varje token:


1. SIGNATUR
   ┌──────────────────────┐
   │ Token säger:         │
   │ "Jag är från BookApi"│
   │ Signatur: "xyz123"   │
   └──────────────────────┘
           ↓
   Backend räknar ut signatur med SecretKey
           ↓
   "xyz123" = "xyz123"? → ✓ Äkta!
   
   Om någon ändrat token → Signaturen stämmer inte 

2. UTGÅNGSTID
   Token: "Giltig till 14:00"
   Nu klockan: 13:30
   13:30 < 14:00 → ✓ Giltig!
   
   Om nu klockan: 15:00
   15:00 > 14:00 →  Utgången!

3. ISSUER (Utfärdare)
   Token säger: "Från BookApi"
   Vi väntar oss: "BookApi"
   "BookApi" = "BookApi" → ✓
   
4. AUDIENCE (Mottagare)
   Token säger: "För BookApiUsers"
   Vi väntar oss: "BookApiUsers"
   Match! → ✓


Lager 3: [Authorize] Attribute

[Authorize]  ← Den här raden säger: "STOPP! Visa nyckel först!
public class BooksController {
    ...
}


Vad händer:

Request kommer in
   ↓
[Authorize] kollar: "Finns token?"
   │
   ├─ NEJ → "401 Unauthorized" (åk ut!)
   │
   └─ JA → ✓ OK, fortsätt till nästa lager

Lager 4: GetUserId()
Läser vem du är från din nyckel


Din token innehåller (kodat):
{
  "NameIdentifier": "42",
  "Name": "johan",
  "Email": "johan@mail.com"
}

GetUserId() läser "NameIdentifier" → 42

Nu vet backend: "Detta är användare 42"


Lager 5: Ownership Check

Kollar att du äger det du försöker ändra:


Du försöker ta bort bok 7
   ↓
Backend:
1. Hitta bok 7 i databasen
   ┌────┬───────┬──────────┐
   │ 7  │ ...   │ ÄgarID:55│
   └────┴───────┴──────────┘
   
2. Jämför: "Äger du (42) bok 7 (ägarID: 55)?"
   42 = 55? → NEJ! 
   
3. Svar: "404 Not Found"

Om det var din bok:
   ┌────┬───────┬──────────┐
   │ 7  │ ...   │ ÄgarID:42│
   └────┴───────┴──────────┘
   
   42 = 42? → JA! ✓
   "OK, ta bort!"




Varför behövs CORS?

Scenario utan CORS:


1. Din webbapp (frontend): http://localhost:4200
2. Din backend (API): http://localhost:5057

Browser tänker:
Hmm, 4200 och 5057 är olika portar...
 Det är som två olika webbplatser!
 Vad om 4200 är en elak sida som försöker
 stjäla data från 5057?
 Jag blockerar detta! 


Scenario med CORS:


Backend säger till browser:
"Hej browser! Det är OK om dessa adresser pratar med mig:
 - http://localhost:4200 ✓
 - http://localhost:4201 ✓
 - http://localhost:63033 ✓"

Browser: "OK, då litar jag på det! ✅"


Hur CORS fungerar

Preflight Request (browser kollar först):


Browser: "Får jag skicka en POST request?"
         ↓
       [OPTIONS request]
         ↓
Backend: "Ja, från localhost:4200 får du!"
         ↓
Browser: "Tack! Då skickar jag riktiga requesten nu"
         ↓
       [Actual POST request]


Vad vi tillåter:


Origins (Från vilka adresser):
✓ http://localhost:4200
✓ http://localhost:4201
✓ http://localhost:63033

Methods (Vilka åtgärder):
✓ GET (hämta)
✓ POST (skapa)
✓ PUT (uppdatera)
✓ DELETE (ta bort)
✓ OPTIONS (kolla om tillåtet)

Headers (Vilka headers):
✓ Authorization (för token)
✓ Content-Type (vad vi skickar)
✓ Alla andra headers



 DEL 8: Komplett Request-Flöde
Scenario: Hämta Dina Böcker

Hela resan från början till slut:**


STEG 1: Du klickar "Visa böcker" i appen
┌──────────────────┐
│ Frontend         │
│ "Hämta böcker"   │
└──────────────────┘
         ↓
         
STEG 2: Frontend gör API-anrop
┌──────────────────────────────┐
│ GET http://localhost:5057    │
│     /api/books               │
│                              │
│ Headers:                     │
│ Authorization: Bearer eyJh...│
└──────────────────────────────┘
         ↓
         
STEG 3: Browser kollar CORS
Browser: "Får 4200 prata med 5057?"
Backend: "Ja! 4200 är tillåten"
Browser: "OK!" ✓
         ↓
         
STEG 4: Request når backend
┌──────────────────┐
│ Backend får:     │
│ GET /api/books   │
│ + token          │
└──────────────────┘
         ↓
         
STEG 5: Token Validation
Backend: "Är token äkta?"
         "Är signatur korrekt?" ✓
         "Inte utgången?" ✓
         "Rätt issuer?" ✓
         "Rätt audience?" ✓
         ↓
         
STEG 6: [Authorize] Check
Backend: "Finns token?" ✓
         "OK, fortsätt"
         ↓
         
STEG 7: Controller kör
BooksController:
1. Läser GetUserId() → 42
2. Frågar databas:
   "SELECT * FROM Books WHERE UserId = 42"
         ↓
         
STEG 8: Databas svarar
Databas: "Här är 3 böcker med UserId = 42"
┌────┬─────────────┬────────┬────────┐
│ 1  │Harry Potter │Rowling │  42    │
│ 2  │LOTR         │Tolkien │  42    │
│ 5  │Gatsby       │Fitzgerald│ 42   │
└────┴─────────────┴────────┴────────┘
         ↓
         
STEG 9: Backend formaterar och skickar
Backend: "Gör om till JSON"
[
  { "id": 1, "title": "Harry Potter", ... },
  { "id": 2, "title": "LOTR", ... },
  { "id": 5, "title": "Gatsby", ... }
]
         ↓
         
STEG 10: Frontend får svar
┌──────────────────┐
│ Frontend visar:  │
│                  │
│ Harry Potter  │
│ LOTR          │
│ Gatsby        │
└──────────────────┘


Tid för hela resan:** ~50-200 millisekunder (snabbare än ett ögonblick!)



En elak person försöker:**

FÖRSÖK 1: Skicka utan token
┌────────────────────┐
│ GET /api/books     │
│ (ingen token)      │
└────────────────────┘
         ↓
Backend: "Ingen token? [Authorize] blockerar!"
         ↓
401 Unauthorized

────────────────────────────────

FÖRSÖK 2: Skicka falsk token
┌────────────────────┐
│ GET /api/books     │
│ Token: "fake123"   │
└────────────────────┘
         ↓
Backend: "Validerar signatur..."
         "fake123" matchar inte rätt signatur
         ↓
401 Unauthorized

────────────────────────────────

FÖRSÖK 3: Skicka utgången token
┌────────────────────┐
│ GET /api/books     │
│ Token: (3 dagar gl)│
└────────────────────┘
         ↓
Backend: "Token giltig till: 01 Dec"
         "Idag är: 03 Dec"
         "Utgången!"
         ↓
401 Unauthorized

────────────────────────────────

FÖRSÖK 4: Ändra ID i token
Hackare försöker ändra:
{ "userId": "42" } → { "userId": "55" }
         ↓
Backend: "Räknar signatur med ändrat innehåll..."
         "Signatur matchar inte!"
         ↓
401 Unauthorized

────────────────────────────────

FÖRSÖK 5: Giltig token men försöker ta annans bok
┌────────────────────────┐
│ DELETE /api/books/3    │
│ Token: giltig (ID: 42) │
└────────────────────────┘
         ↓
Backend: "Token OK ✓"
         "Vem är du? ID: 42"
         "Hitta bok 3..."
         "Bok 3 ägs av ID: 55"
         "42 ≠ 55"
         ↓
 404 Not Found (boken "finns inte" för dig)


DEL 9: API Endpoints (Översikt)


ÖPPNA (Ingen token krävs)
├─ POST /api/auth/register    → Skapa konto
└─ POST /api/auth/login        → Logga in

 SKYDDADE (Token krävs)
│
├─ BÖCKER
│  ├─ GET    /api/books           → Alla dina böcker
│  ├─ GET    /api/books/{id}      → En specifik bok
│  ├─ POST   /api/books           → Skapa ny bok
│  ├─ PUT    /api/books/{id}      → Uppdatera bok
│  └─ DELETE /api/books/{id}      → Ta bort bok
│
└─ CITAT
   ├─ GET    /api/quotes          → Alla dina citat
   ├─ GET    /api/quotes/{id}     → Ett specifikt citat
   ├─ GET    /api/quotes/book/{id}→ Citat från en bok
   ├─ POST   /api/quotes          → Skapa nytt citat
   ├─ PUT    /api/quotes/{id}     → Uppdatera citat
   └─ DELETE /api/quotes/{id}     → Ta bort citat






 Hela Systemet i Ett Nötskal


1. ANVÄNDARE registrerar → Får JWT token
2. ANVÄNDARE loggar in → Får ny JWT token
3. Varje request → Skickar token i header
4. Backend validerar → Token OK?
5. Backend läser → Vem är du? (från token)
6. Backend filtrerar → Bara din data
7. Backend svarar → Dina böcker/citat


Säkerhetsgarantier
Du kan **aldrig** se någon annans data  
Du kan **aldrig** ändra någon annans data  
Du kan **aldrig** ta bort någon annans data  
Token går ut efter 24 timmar (måste logga in igen)  
Lösenord lagras aldrig i klartext (alltid hashat)  

Key Concepts

WT Token = Din magiska nyckel  
UserId = Ditt unika ID-nummer  
[Authorize] = "Visa nyckel först!"  
GetUserId() = Läs vem du är från nyckeln  
Ownership Check = "Äger du detta?"  



