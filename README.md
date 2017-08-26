# Tower Defense Random Levels Generator

The project allow you to generate random levels for a tower defense game made in Unity.
You can define for each level:

* the **size** of the grid
* the **start** point and the **end** point of the path generated
* the **colors** of the background and trail

<p align="center">
  <img width="460" height="420" src="https://github.com/Lucci93/TDRandomLevelsGenerator/blob/master/store-assets/slide.gif">
</p>

## Contributing

If you are interested in fixing issues and contributing directly to the code base or you want to request a new feature:

* contact me
* report an issue
* create a pull request.

### Prerequisites

You must have at least **Unity 6.x.x** installed to run the project, however, **Unity 2017.1** is recommended.

### Installing

Download or clone the repository and launch the *Main.unity* file inside the *Asset/Scenes* folder.

## Running the tests


Add all the levels you want in the editor and after pressing *play*, press *N* on the keyboard to generate the next level.
Before you begin, make sure you **have correctly set the start and end points** of the route, so that you have:

```C#
* start.x > 0
* start.y > 0
* end.x <= size.x-2
* end.y <= size.y-2
```

## Built With

* [Unity 2017.1](https://unity3d.com)
* [Visual Studio For Mac](https://www.visualstudio.com/vs/visual-studio-mac/)

## Authors

**Daniele Piergigli** - *A bored student of computer science* - [Lucci93](https://github.com/Lucci93)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
