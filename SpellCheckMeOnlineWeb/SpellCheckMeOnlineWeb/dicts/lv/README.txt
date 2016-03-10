# Latvie�u valodas pareizrakst�bas p�rbaudes bibliot�ka (afiksu un v�rdn�cas fails) 
# lieto�anai ar OpenOffice 2.4.1 un augst�k
# Latvian spelling dictionary (affix and dictionary files) for OpenOffice 2.4.1 and higher
#
# Copyright (C) 2002-2010 Janis Eisaks, jancs@dv.lv, http://dict.dv.lv
# 
# �� bibliot�ka tiek licenc�ta ar Lesser General Public Licence (LGPL) 2.1 nosac�jumiem. 
# Licences nosac�jumi pievienoti fail� license.txt vai ieg�stami t�mek�a vietn�  
# http://www.fsf.org/licensing/licenses/lgpl.html
# 
# This library is free software; you can redistribute it and/or
# modify it under the terms of the GNU Lesser General Public
# License as published by the Free Software Foundation; either
# version 2.1 of the License, or (at your option) any later version.
#
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
# Lesser General Public License for more details.
#
# You should have received a copy of the GNU Lesser General Public
# license along with this library; if not, write to the Free Software
# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA


1. Uzst�d��ana
2. Interesentiem
3. Izmai�u saraksts

Svar�gs pazi�ojums:

�� v�rdn�cas versija vairs neuztur MySpell pareizrakst�bas dzi�a izmanto�anu.
Afiksu b�ze ir veidota, izmantojot Hunspell pala�in�to funkcionalit�ti.

=================

1. V�rd�cas uzst�d��ana

Ieteikums: uzst�d�t vismaz OO 3.2 versiju.
V�rdn�cas uzst�d��ana ir �oti vienk�r�a - izmantojot OO Extension Manager.
Extension Manager pied�v� iesp�ju k� tie�saistes, t� lok�lu papla�in�jumu uzst�d��anu.
Ja uzst�d��ana tie�saistes re��m� nav iesp�jama, vajadz�go valodas papla�in�jumu (v�rdn�cu)
var lejupiel�d�t �eit:

http://extensions.services.openoffice.org/dictionary

un izmantot lok�lai uzst�d��anai.

Ja izmantojat OO versiju, kas neuztur Extensions (pirms 2.4.1), tad:

1. iesp�ja. Uzst�d��ana tie�saistes re��m�
 No izv�lnes File/Wizards/Install new dictionaries palaidiet att. vedni, izv�lieties 
 Jums t�kamo ved�a valodu un sekojiet nor�d�jumiem. Bez latvie�u valodas pareizrakst�bas 
 r�kiem J�s vienlaic�gi varat uzst�d�t papildus valodas vai atsvaidzin�t eso��s bibliot�kas.
 (Uzman�bu! - nav zin�ms, cik ilgi �� bibliot�ka v�l tiks aktualiz�ta; pilns laidienu arh�vs ir 
  atrodams http://sourceforge.net/projects/openoffice-lv/)

 Ja kaut k�du iemeslu d�� nevarat izmantot 1. iesp�ju, ir
 
 2. iesp�ja. "Offline" uzst�d��ana
 Lejupiel�d�jiet p�d�jo modu�a versiju no openoffice-lv.sourceforge.net .
 P�c faila ieg��anas tas ir j�atpako direktorij� %Openoffice%\share\dict\ooo, 
 kur %Openoffice% - direktorija, kur� veikta OpenOffice uzst�d��ana. Tur eso�ajam failam 
 dictionary.lst ir j�pievieno sekojo�as rindas: 
 
 DICT lv LV lv_LV
 HYPH lv LV hyph_lv_LV

 vai ar� j�izpilda win-lv_LV_add.bat (Windows gad�jum�) vai, Linux gd�jum�, j�izpilda 
 komandu:

   sh <lin-lv_LV_add.sh

 Lai izpild�tu 2. iesp�ju, Jums ir j�b�t ties�b�m rakst�t min�taj� katalog�. Ja t�du nav, 
 varat uzst�d�t v�rdn�cu lok�li, sav� lietot�ja opciju katalog� (%OOopt%/user/wordbook).

 Offline uzstad��anai var izmantot ar� 1. iesp�j� min�to vedni, viss notiks l�dz�gi, 
 tikai nepiecie�amaj�m modu�u pakotn�m b�s j�b�t uz lok�l� diska. J�piez�m� ka, piem�ram, 
 SUSE gad�jum� min�tais vednis ir izgriezts �r� no OO un 2. iespeja ir vien�g�. Atsevi��i 
 �is l�dzeklis un v�rdn�cas ir ieg�stams t�mek�a vietn�
 
  http://wiki.services.openoffice.org/wiki/Dictionaries

Ar to modu�u uzst�d��ana praktiski ir pabeigta; atliek vien�gi caur 
Options>Language settings>Writing aids iesl�gt vai izsl�gt nepiecie�amos modu�us un 
iestat�t dokumentu noklus�to valodu.


 Ja ir nepiecie�ama autom�tisk� pareizrakst�bas p�rbaude, zem Tools>Spellcheck j�ie�eks� 
 AutoSpellcheck.

================

2. Interesentiem

Ja jums ir iekr�ju�ies v�rdi, kurus �is l�dzeklis neatpaz�st vai ar� atpaz�st k��daini, esat
laipni aicin�ti tos ats�t�t t�l�kai v�rdn�cas pilnveido�anai vai ar� re�istr�ties v�rdn�cas 
izstr�dei velt�taj� vietn� //dict.dv.lv.

Liel�ka apjoma dokumentu filtr��anai var izmantot sekojo�� viet� atrodamu StarBasic makrosu:
http://lingucomponent.openoffice.org/servlets/ReadMsg?listName=dev&msgNo=1843

Piez�me - makross nedarbojas ar OO >3.0.

Sarakstu gad�jum� ir l�gums s�kum� pa�iem kritiski izv�rt�t neatpaz�to v�rdu pareiz�bu 
vai to pielietojam�bu (piem slengs, barbarismi utml. drazas, manupr�t, nav t� v�rtas, 
lai t�s iek�autu pareizrakst�bas p�rbaudes v�rdn�c�, lai gan viena otra tom�r iespraucas).
