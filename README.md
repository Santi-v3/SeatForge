# SeatForge

SeatForge ist ein eigenständiger Clean-Room-Prototyp für eine lokale Multi-Seat-Verwaltung unter Windows.

## Aktueller Umfang

- Liest relevante Eingabe-, Anzeige-, Kamera- und Audiogeräte über WMI.
- Ordnet Geräte in der Oberfläche „Seat 1“, „Seat 2“ oder „Unassigned“ zu.
- Filtert Geräte live nach Name, Kategorie, Hersteller und Sitz.
- Speichert ausschließlich die Zuordnung lokal unter `%LOCALAPPDATA%\SeatForge\assignments.json`.
- Installiert keine Treiber und verändert weder Anmeldung noch Registry oder Windows-Sitzungen.

## Starten

```powershell
.\Run.ps1
```

Das Projekt enthaelt lokal eine portable .NET-8-SDK-Kopie unter `.tools/`, die
nicht zu GitHub hochgeladen wird. Alternativ funktioniert auch `dotnet run`,
wenn .NET 8 systemweit installiert ist.

## Bauen und pruefen

```powershell
.\Build.ps1
```

## Geplante Phasen

1. Geräteinventar und lokale Sitz-Zuordnung.
2. Robuste Geräte-IDs, Vorlagen sowie Import/Export.
3. Separater, nicht privilegierter Hintergrunddienst für Geräteereignisse.
4. Erst nach technischer und rechtlicher Prüfung: experimentelle Windows-Sitzungs- und Treiberarbeit in einer VM.

Hydra-Code, Markenmaterial und Lizenzmechanismen werden nicht übernommen.

## Kostenlose Coding-Agenten

Die Projektdatei `opencode.json` bindet die anonyme Gratis-Route von Kilo Code in OpenCode ein. Verfügbare Beispiele:

```powershell
opencode run --model kilo-code/kilo-auto/free "Beschreibe deine Aufgabe"
opencode run --model opencode/glm-5.3-flash-free "Beschreibe deine Aufgabe"
freebuff
```

Die Anbieter und Gratis-Modelle wechseln. Als aktuelle Quelle dient
[`awesome-free-ai-coding`](https://github.com/mvalentsev/awesome-free-ai-coding), das seine Einträge automatisiert prüft.
FreeBuff, Kilo und manche kostenlose OpenCode-Modelle dürfen übermittelten Code speichern oder zum Modelltraining verwenden.

Fuer projektuebergreifende Aufgaben kann der separate
`Santi KI Router` Claude, Codex und kostenlose Worker nach Schwierigkeit einsetzen.
