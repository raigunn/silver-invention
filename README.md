# silver-invention
Sitecore Google AMP Module

Silver Invention is intended to be an open source bootstrap for making Google AMP enabled pages in Sitecore.

## There are 4 initial areas I hope to tackle:
1) Boilerplate Layout
2) CSS
3) Markup Conversion
4) Schemaa.org Structured Content

## Boilerplate Layout
The current plan is we will make an AMP based MVC layout that has the boilerplate requirements like 'amp' in the `<html>` tag etc.  
See: https://www.ampproject.org/docs/tutorials/create/basic_markup

There will be a Sitecore Device that is triggered with `amp=1` which can be used to set the presentation details using the AMP layout.

## CSS
There is a requirement that there can be no references to external includes.  This means all of the CSS for the entire page must be within `<style>` tags.  The plan for this bootstrap is to have a rendering that will read the css from a single file and load it into a `<style>` tag in the `<head>` section.  The file with the css could be one generated from a sass pre-compiler, that is up to you.
See: https://www.ampproject.org/docs/guides/responsive/style_pages

## Markup Conversion
There are some simple atomic structures that need to be converted.  Such as swapping <img> tags with <amp-img> tags.  These are fairly straightfoward.  However there are others which are more molecular, such as in responsive design you may be using `<picture>` tags like this:
```
<picture ...>
  <source .../>
  <source .../>
  <source .../>
  <img .../>
</picture>
```
These need to be changed to a series of `<amp-img ...></amp-img>` tags.  For more information see: https://www.ampproject.org/docs/guides/responsive/responsive_design#changing-the-art-direction-of-an-image

There are several approaches I'm considering for the markup conversion.

a) Get all of the markup for a page after MVC has put it all together, and before it is rendered run a conversion script.

b) Extend controls that normally return `<img>` tags so they can contextually return `<amp-img>` tags

c) Create a alternate markup for each of the components on the site.

### Pros/Cons
With a) it's kind of a silver bullet.  The content author and front-end folks don't need to do anything different.  This conversion script will come sweeping in, in a nick of time to convert all the markup to the appropriate AMP representations.  The challenge is we have to write the conversion code, which in the case of the `<picture>` tags can be messy.  

With b) we would need to extend how Sitecore renders image fields.  We may also need to extend how glassmapper extends image fields.  And even so, there is the looming rich text fields that will still need a conversion to run. 

With c) to create two versions of every component could be burden on the front-end devs.  Could also lead to maintanence and testing issues.  And we still may have issues with both rendering image fields coming from Sitecore and with image tags in rich text.

So for the purposes of this POC, we're going to go with option a, and create a silver bullet post-processing / pre-rendering conversion script.

## Schema.org Structured Content.
For this POC we are going to use the recommended json+ld format.  See this page for more information:  https://developers.google.com/search/docs/guides/intro-structured-data

The plan for this POC is to have a base template for each content type with the necessary schema.org fields in it.  We should have a few different representative content types such as Article, Event, Press Release, etc.  And then we'll need a rendering that pulls these fields from the template and places them into the `<head>` section in the appropriate json+ld format.

# Contribute
Contributions and suggestions would be great.  Please submit a pull request.  





