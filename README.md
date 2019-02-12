# Sqwiki
Obtiene los ¿Sabías que...? de Wikipedia en español que contengan una imagen en la página principal.

Requiere la dll de [MWBot.net](https://github.com/MarioFinale/MWBot.net) en el mismo directorio de ejecución.
Tambien requiere un archivo de configuración de MWBot.net, en caso de que no exista, puede crearse con el asistente en modo consola.

# Uso
El programa busca un archivo llamado sqwiki.runme en el directorio de ejecutable. Al encontrar el archivo lo borra, crea la carpeta /sqs y coloca en ella los SQ.


El formato de los SQ dentro de la carpeta es el siguiente:
- [N° de SQ].htm (contiene un documento html simple con el texto del SQ más un botón para copiarlo).
- [N° de SQ].png (contiene una imagen .png correspondiente a la imagen del artículo principal del SQ).
- [N° de SQ].c.htm (contiene el enlace al archivo de origen en commons).
