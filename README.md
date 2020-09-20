# Plánovač zakázok

Aplikácia ktorá eviduje firemné zakázky. Každá zakázka má pridelenú pobočku a miesto výkonu úlohy. Z BingMaps sťahuje mapu na ktorej vyznačí obidve adresy pushpinmi. Druhé okno ukazuje prehľad pobočiek firmy a zakázky príslušné k tejto pobočke. Obidve okná sú synchronizované. Keď zmením na zakázke jednu alebo druhú adresu, prehľady sa aktualizujú a z webu sú stiahnuté nové mapy. Klient volá webovú službu ktorá ukladá data na Sql Server Express.
