# ![MixEmul](http://rbergen.home.xs4all.nl/mixemullogo.jpg)MixEmul
## What is it?
MixEmul is an emulator for the MIX computer that is described in The Art of Computer Programming (TAOCP) series of books from D.E. Knuth. MIX is a mythical, non-existent computer with features similar to those of real computers of the 1960s. It ‑ or more accurately, its assembly language MIXAL ‑ is used as the foundation for the text of aforementioned books.
MixEmul completely emulates the entire MIX instruction set. With that I mean that MixEmul not only performs the actions that an instruction should ‑ like multiplying when it encounters a MUL instruction ‑, but it does it in the way MIX would. For example, the time unit ("tick") counts that are included in TAOCP Volume 1 are implemented as well. Also, as TAOCP specifies, I/O operations are performed in the background.

Speaking of I/O, all 21 MIX I/O devices (tapes, disks, card reader, card punch, printer, teletype and papertape) are implemented in MixEmul.

MixEmul lets you edit the contents of MIX's registers and memory before, during and after program execution using a number of editor types and review device status in a single glance. It includes a breakpoint feature and allows programs to be run in the background. It includes the GO button functionality, supports interrupts, comes with the floating point module, performs execution profiling and it’s even mildly configurable.

Oh, I'd almost forget, it incorporates a MIXAL assembler too. This means that you can write MIXAL programs using any text editor you like and then load them into MixEmul to debug and run them.

In a few words: MixEmul contains all the features that I think I need to be able to better absorb the contents of the TAOCP books.
## Can I get it?
Well, obviously the answer is yes; you're looking at its sources as we speak. However, getting it to run doesn't even require you to build it yourself. The latest binary version I have seen working myself can be downloaded from [this location](http://rbergen.home.xs4all.nl/mixemul.html).
## What's with all the security warnings?
For incomprehensible reasons, MIX enthusiasts are not as common as you or I would like. This means that MixEmul is not downloaded as often as, for instance, Google Chrome. Furthermore, I loved writing MixEmul and I love maintaining it, but out of principle I refuse to cash out for things like code signing certificates. This means that on an average computer, your browser, your OS and probably your virus scanner will all object to downloading and running MixEmul.
I'm afraid you'll just have to trust me when I say that MixEmul was not written to cause you or your computer any harm. Over the years, I have spent quite a few hours figuring out first how to improve MixEmul and then how to code those improvements; causing trouble is never an improvement in my book.

So again, trust me when I say that MixEmul is safe. Or don't trust me, and miss out on what I still consider to be the best MIX emulator available. :)
