# Task
It is necessary to implement the library and the console program using it to create a local copy of the site (“analogue” of the [wget](https://ru.wikipedia.org/wiki/Wget) program) 

## Program / Library Options:
- restriction on the depth of link analysis (i.e. if you downloaded the page that the user specified, it is level 0, all pages to which will enter links from it, it is level 1, etc.)
- restriction on the transition to other domains (no restrictions / only within the current domain / not higher than the path in the source URL)
- restriction on the “expansion” of downloaded resources (you can set a list, for example: gif, jpeg, jpg, pdf)
