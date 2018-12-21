# Interview Challenges #

### Overview ###

For a long while, the prevailing mindset when looking to hire technical candidates was that the most effective way to evaluate their problem solving and coding skills was to present programming challenges to be solved on a whiteboard.  To many, this is an alien way to code and is not likely to be a skill used outside of interviewing.  The challenges themselves are often not reflective of the day-to-day technical work and frequently either hearken back to theory taught in college or are designed to require candidates to figure out a specific trick to solve them. 

Throughout the course of my career, I've found myself in this setting a fair number of times and have never gotten comfortable.  After an interview was complete and I had time to reflect on the problem and my solutions, I'll often have that "ah-ha" moment, coming away with a number things that didn't occur to me during the interview, and ideas to improve my answers.  

This repository is meant to capture some of these challenges, detail thoughts around solving them, and implement solutions.  The spirit of the repository is not to offer an interview cheat sheet, but rather to give myself opportunity to revisit things that vexed me, work through them in my own time, and to implement them following good practices rather than whiteboard scratch efforts.  Solutions herein  aren't necessarily the most optimal, or the "textbook" answers that interviewers were looking for, but should indicate a practical approach and a rational thought process behind them.  

### Goals ###

- Examine a set of interview challenges, each a self-contained project with no co-dependencies. 

- Document each challenge fully, including the problem, solutions, implementations, and any relevant theory or guidance for each.

- Allow for multiple solutions to each problem, allowing for exploration of different approaches and technology stacks.
  
- Provide a professional level of polish for each implementation, following language idioms and practices.

- Ensure that each implementation is accompanied by the appropriate automated tests, for technology stacks that support it.

### Structure ###

* **src**
  <br />_The container for project source code.  Each challenge will have its own area under this container with its documentation and implementations._

* **.editorconfig**
  <br />_The standardized [editor configuration](https://editorconfig.org/) for defining and maintaining common conventions for code across developers.  This configuration is intended to serve as the default for all implementations and span technology stacks.  In the event that a specific implementation has special needs, a local override will appear in its container._
  
* **.gitattributes**
  <br />_The root configuration for the treatment of assets within the source control system.  This is intended to provide a baseline for each challenge and includes a wide set of languages and technology platforms._

* **.gitignore**
  <br />_The root configuration for assets that should be ignored by the repository.  This is intended to provide the baseline for each challenge.  As a result, an effort was made to include a wide variety of languages and technology stacks.  In the event that a specific implementation has special needs, a local override will appear in its container._
  
* **CONDUCT.md**
  <br />_The code of conduct expected by those contributing and participating in the community of this repository._
  
* **CONTRIBUTING.md**
  <br />_The guidelines for contributing to this repository._
  
* **LICENSE.md**
  <br />_The license applied to the assets and ideas contained in this repository.  In the event that a specific implementation has special needs, a local license will appear in its container._
    
### Licensing ###

The artifacts in this repository are offered under the MIT license as described in the accompanying [license](./LICENSE "license") file, unless a local license is present for a specific implementation, in which case the local license takes precedence.  Feel free to use the code in your own projects or the documents as templates as you see fit.  Should these works prove helpful or inspire you to iterate on them in creative ways, I find that to be a huge compliment.  We all build upon the work of those who have come before us.  In general, formal attribution is not necessary, though it is always appreciated.  I do ask, however, that you not copy an item verbatim and pass it off as your own work.  