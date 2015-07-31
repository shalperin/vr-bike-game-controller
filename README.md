[![DOI](https://zenodo.org/badge/16209/shalperin/vr-bike.svg)](https://zenodo.org/badge/latestdoi/16209/shalperin/vr-bike)

#VR Bike #
By Sam Halperin

##Safety Note##
Please note: My expertise is software engineering, not EE.  If you see something grossly unsafe or otherwise wrong here, please email me at <a href="mailto:sam@samhalperin.com">sam@samhalperin.com</a>

##Contents of this repo##
+ **arduino sketches**:  For debugging hardware while it is being built.  Once it's working with these sketches upload the standard firmata so that the software can communicate with it.
+ **circuit diagrams**
+ **parts list**
+ **images of sensor**: snapshots of built versions of the sensor.
+ **unity**:  Currently contains two fairly blank race track scenes.  One configured for an Oculus Rift, the other configured for desktop display.  See **unity/Assets/CycleVR** 

##Software Required to work with this code##
+ Unity (Pro required for Oculus Rift Build)
+ Unidunio (From asset store.  I think it's $10)
    * You should up with Uniduino folder in assets
    * Don't forget to run Uniduino port configuration (Window > Uniduino)
+ Oculus VR Unity Support
    * Should end up with OVR folder in Assets.



# Exploring Bicycle-Based Virtual Reality Exergames as a Design Space
<p>
	<a href="http://www.samhalperin.com/img/projects/vr-cycle/2015-01-06-poster.pdf">
		<img src="http://www.samhalperin.com/img/projects/vr-cycle/poster-thumbnail.png" /></a>
</p>
<h2>Abstract</h2>
<p>With the introduction of devices like the Oculus Rift, Samsung Gear, and Google Cardboard, immersive virtual reality experiences are emerging from research laboratories into mainstream use.  These experiences are characterized by the sense of presence that they create, putting participants visually, viscerally and psychologically into imagined, historical, representational, and impossible environments.</p>

<p>Movement bases experiences, particularly those that combine bicycles, treadmills, and body tracking with VR, are one area of focus for HCI research.  The exploration of game dynamics, the specific game play elements within the system contexts (e.g. limited sensors) and human contexts (e.g. fatigue) of movement could lend itself to a design science approach.</p>
<p>The paper suggests a number of new game dynamics at the concept level, and explores one game dynamic, the non-linear mapping of RPM sensor data to camera movement in more detail.  This expanding set of designs within the VR-exergaming design space  is proposed as a direction for future research.</p>

<h2>Acknowledgements</h2>
<p>This paper was written during HCI at Nova Southeastern University under the supervision of Dr. Laurie Dringus.</p>

<h2>Background and Context</h2>
<p>The research reviewed below defines the boundaries of the virtual reality exergaming design space.  The experiments lay out the basic human and system constraints in VR and exergaming, including simulator sickness, occlusion and limited resolution (Steinicke &amp; Bruder, 2014), ambiguous sensor data, managing fatigue, and managing movement’s cognitive load. (Mueller &amp; Isbister, 2014).</p>
<p>In the popular press and in product marketing materials, the characterization of an experience as ‘immersive’ has been widely applied to devices that provide an engaging experience but do not fully occlude or replace the participant's view of the world with computer generated imagery.  Head mounted displays uniquely immerse the user in generated environments.  The detractors of these interfaces have described them as “gluttonous” (Weiser 1994), but others describe them in the historical context of apparatuses like the ‘lanterna magica’ as speaking to a primal human experience of fantasy and transportation (Carvalho 2013).</p>
<p>	Modern devices like the Oculus Rift, a moderate resolution, wide field-of-view pair of goggles that a user wears, produce compelling experiences, centered on the experience of ‘presence’. Presence is “the subjective experience of being in one place or environment, even when one is physically situated in another”  (Witmer &amp; Singer, 1998). The Oculus Rift, and devices of its class, uniquely creates this sense of presence where viewing on a TV or tablet does not.</p>
<p>There are myriad applications of virtual reality technology including, but not limited to: architectural visualization, collaboration and communication, entertainment, and storytelling.  Because research on ‘presence’ was limited to a relatively small set of VR labs in the past, ‘applications of presence’  to a larger set of problems is a relatively new field.  As evidenced by communities like the Philadelphia and New York VR Meetups, this is primarily driven by the wider audience of experimenters that have access to working with the phenomenon.</p>
<p>VR Bicycle based experiences, one class of applications that center on exertion, have been extensively studied.  Mestre, Dagonneau, and Mercier (2011) indicated that this type of experience does effectively motivate exercise.   While we know in a coarse way that the VR interface itself might contribute to enjoyment and motivation from this study, less is known about the characteristics (particularly in terms of game dynamics) that delineate effective VR exergaming experiences from less effective ones.</p>
<p>
	<img src="http://www.samhalperin.com/img/projects/vr-cycle/vr-bike-island.gif"></img></p>
<p><p>Exergames are experiences that intend to include exertion as part of  their human physiological context.  Movement based games must engage in the active management of fatigue (Mueller &amp; Isbister 2014).  Techniques for evaluating movement games include the measurement of heart rate , as well as subjective instruments focused on evaluating perceived exertion (Huang, Tsai, Sung,  Lin, &amp; Chuang,  2008).</p>
<p>VR is not without its challenges.  Primarily, fully occlusive virtual environments can create a ‘simulator sickness’ (SS) in many users. (Steinicke &amp; Bruder, 2014).  SS is caused by a visual/vestibular mismatch between what is seen on the VR display and what the inner ear and proprioceptive human systems experience (Zhang, Li, Kuhl, 2014; Steinecke &amp; Bruder, 2014).    </p>

<h2>Problem and Research Questions</h2>
<p>While basic parameters of VR exergaming such as SS, ambiguous sensors, presence, exercise motivating qualities of VR interfaces, and the management of fatigue (Maestre et. al., 2011; Mueller & Isbister 2014), not enough is known about the specific game play dynamics that define well designed artifacts within this design space.</p>
<p>Gaver (2012) describes ‘design science’ as a process of the ‘generative’ creation of artifacts to explore a design space, and the annotation of those artifacts to describe a theory around the work.  Often this work is done in the context of user-centered design, with feedback from user studies informing the design process .   Read in conjunction with Buxton (2010), who articulated ideas on the sketching process (as different from prototyping process) for generating user interface designs (2010), and expanding the designers toolkit to include game engines that allow rapid prototyping, it is easy to see an approach for design research based on ‘making’ in this field.</p>

<h3>Research questions could include:</h3>
<ul>
<li>What can be learned  about creative mappings of RPM pedaling data to game dynamics such as riding speed, airplane throttle, point accrual, player health, shields and weapons.  In particular, non-linear mapping from RPM to these should be explored with the goal of, as Mueller &amp; Isbister (2011) suggest, creating fun and well articulated controllers.</li>
<li>How can the phenomenon of “presence” be leveraged effectively for exergames.  Questions to be answered could center around whether riding/driving is the most compelling dynamic for an exergame, particularly with considerations for simulator sickness.  This could be explored through the creation of prototype experiences that do not center on riding.  (Work that was started with the VR/Exergaming poster and skeet shooting example at NSU Fall 2015 poster session.)</li>
<p>
	<img src="http://www.samhalperin.com/img/projects/vr-cycle/virtual-led.png"></img>
</p>
</ul>

<h2>Literature Review</h2>
<p>Bicycle based VR games universally map RPM pedal cadence to some aspect of game play.  It is understood from movement game theory and from other domains such as redirected walking that direct, linear mappings of sensor data can be problematic in terms of ambiguity of the data and also in terms of a missed opportunity to creatively map movement to game dynamics (Mueller &amp; Isbister 2014, Zhang et. al., 2014).</p>
<p>Experimenters have used rotary encoders (Park, Lee, MacKenzie, Moon, Hwang, &amp; Song 2014) and magnet activated reed switches (used in my own experimentation and on traditional handlebar mounted bicycle computers) to count the timing of pedal and wheel revolutions in stationary apparatuses.  Some design based research has started to use this data to produce interesting and entertaining experiences. (See for example “PaperDude”, a movement-based clone of an old Atari video game by Bolton, Lambert, Lirette, &amp; Unsworth, 2014).  However not enough is known about game dynamics that use RPM data in creative ways to create exercise-motivating experiences.</p>
<p>Video and auditory stimulus, the sum of game dynamics rendered for the senses, simulating a real riding experience has been demonstrated to be an effective motivator in some studies (Mestre et. al., 2011).    Other studies have shown that virtual reality, as compared to no stimulus or television based stimulus, uniquely drives heart rate response in riders (Huang et. al., 2008).  The combination of the possibility to both motivate people to work out, and to shape their workouts in terms of physiological response is compelling.</p>
<p>The dynamics unique to movement-games center on a few fundamental aspects of that type of experience, and include: a) embracing movement and sensor ambiguity, b) celebrating the articulation of movement (i.e. by trying to make movement based on active controllers as expressive as possible within the constraints of limited data, c) intending and actively managing fatigue, d) highlighting rhythm, and e) incorporating social experiences, among others. (Mueller &amp; Isbister 2014).    A recent study that used a simple RPM/Tachometer graphic  to study the Fitt’s law performance of bicycle controllers (Park et. al., 2014, see below) is characteristic of the gulf between the needed research for producing compelling fitness games and the reality of the current exploration of the design space.</p>
<p>Specific to exercise bicycles as game controllers, Park’s comparison of the performance of directly mapped speed based sensors has yields an understanding of how these controllers conform to existing theory around pointing task efficiency  (Park et. al., 2014).  In the study, it was observed that game controllers based on pedaling velocity, when measured in pointing tasks, do seem to adhere to the same Fitt’s task performance as other pointing tasks.</p>
<p>However, in addition to the coarse or nonexistent game dynamics in the Park study, also not very well covered was the impact of fatigue on cadence based controller usage.  When the error rate between a pointer target and actual pointer movement was studied, at high levels of exertion the error rates were all due to falling short of exertion goals.  It is possible that controller articulation could be improved at higher excretion rates by making the algorithms driving pointer movement more sensitive at higher levels of exertion.  The point would be to both encourage higher levels of exertion and to make sure that the controls remain well articulated at the higher output.  This is only as complicated as getting an exponential mapping of sensor data to operate in the same range as a linear mapping, work that I have already started to experiment with in the context of the existing rig.</p>
<p>	The idea that motion data should be interpreted non-linearly arises in both movement theory gaming research, falling under the category of ‘taking advantage of ambiguities in movement’ and ‘celebrating articulation’ (Mueller &amp; Isbister 2014)  as well as virtual reality research more focused on applications like ‘redirected walking’, a process of modifying user inputs so that walking movements can explore a large virtual space using a constrained laboratory environment. (Zhang et. al., 2014).
Redirected walking is outside of the scope of this paper, but is a good example of how abstractions from real movement yield interesting results in VR.	The outcome of the work by Zhang et. al. reflects an opportunity to creatively map sensor data in an exergame, as they noted that participants do not universally notice modifications of sensor data from the real.  Both the ambiguity of sensor data, and the possibility of creating a more interesting experience are well understood in the movement gaming research.</p>

<p>
	<img src="http://www.samhalperin.com/img/projects/vr-cycle/main.png"/>
</p>

<p>Like many other exergaming sensors, sensor data from low-cost stationary bicycle based apparati can be imprecise.  Even where a direct mapping from RPM to camera velocity is possible, it may not be desired. (Mueller and Isbister 2014).</p>

<p>	Studies on VR exposure such as the one done by Steinicke and Bruder (2014) seem grounded in the cyberpunk fiction of the 1980’s and 1990's.  One could question the motivation to stay in VR for 24 hours, and apply that same doubt to ‘digitizing’ the riding experience.   By imerrsing a user in VR for a long period of time, their experiment informs some of the long-term human challenges with this type of interface, particularly as their participant experienced simulator sickness while moving around the virtual environment.</p>
<p>	Game dynamics created in the human context of SS will have to take into account camera platform movement in a conscientious way.  Acceleration of the platform (if indeed it moves at all) might be an experience that is targeted at experienced VR participants rather than neophytes.  Instruments for assessing SS are covered below.</p>

<p>Immersive VR speaks to some very primal instinct towards fantasy, towards shaping our environment and existing within wholly built and and imagined spaces (Carvalho 2013).  Just as the smartphone and social media website have changed our worldview, so will VR also reshape the way people interact.</p>
<p>For urban users, experience characterized by smartphones, tablets, computers and game consoles, is already highly electronically mediated. The chance to ride a bicycle represents the addition of movement to an existing electronic/sedentary system, not the further detachment of real experience from otherwise natural environments.</p>
<p>Virtual coaching has been identified as an area of game dynamics requiring further study (Richard 2014).  Experience from team sports instructs that the input of wise leadership, and the interaction between that leadership and a community of participants is a compelling motivator (in both young and masters level participants).   To some extent we can modify the idea of virtual coaching suggested by Richard, to include opportunities afforded by the game environments themselves.  For example, coaching / goal setting input might be fairly simple, such as ‘2 loops around the tropical island environment at a moderate pace.’  (The tropical island environment was a recent enhancement to the work presented at the NSU poster session.  Additionally timing data that might measure pace is within reach.)</p>

<p>Turning now to the actual process of game design, one approach detailed by Mueller (Mueller, Gibbs, Vetere, and Edge, 2014) was the use of ‘exertion cards’ to design the game dynamics.  The cards generally get at the aspects of movement games in Mueller’s paper on movement games principles, such as the management of fatigue, or the leveraging of play-player communication (Mueller &amp; Isbister 2014)</p>
<p>Generally, a programming/prototyping based approach has seemed to produce more tangible results, but what Mueller’s process hints at is a layered sketching process that begins with pen and paper, might include some user testing at that level, and then is successively refined as a design, moving towards a playable prototype.  This is an important process note, because as the designer moves from loose sketch to playable prototype, the commitment to individual designs increases.  Generally it is possible to iterate on designs at a late stage, even in a game-engine based prototype, if good practice around reusability and modularity is adhered to. Generally these prototypes provide higher fidelity information about the design space, but low fi ‘paper prototyping’ is an important technique.</p>
<p>These artifacts will be best articulated as working demonstrations implemented using tools that allow for rapid prototyping.  (See for example, Unity Game Engine.)    Sketching in the Buxtonian tradition (Buxton 2010) should be informed and guided (if not constrained) by the science of what is possible to implement in a demo.    The annotation, theory and discussion that arises from said work is what separates these design artifacts from what Buxton calls “slick demos”.  Necessary in this process is the capacity to quickly create a lot of solutions in the design space.</p>

<p>	Finally, future work will want to evaluate any artifacts with respect to how they interact with the human contexts of their use.  Instruments for measuring simulator sickness, game enjoyment, presence, and perceived exertion will need to be applied.   A cursory review of existing instruments yields the following:</p>
<ul>
<li>The Witmer and Singer Presence questionnaire (Witmer &amp; Singer 1998)  This 32 point questionnaire subjectively measures the experience of presence after participating in an experience.</li>
<li>The above presence questionnaire  can be used in conjunction with the Immersive Tendency Questionnaire (Witmer &amp; Singer 1998) to try to normalize results for participants different propensities for becoming immersed in regular experiences.</li>
<li>Steinike also used the Slater-Usoh-Steed (SUS) Presence Questionnaire in their study. (Usoh, Catena,  Arman, and Slater, 2000 cited in Steinicke &amp; Bruder 2014)  This test is applicable to experiments within an interface meme (VR exercise bicycling) but may not be useful for comparing different memes (VR biking versus TV biking).</li>
<li>For measuring Simulator Sickness, the Kennedy-Lane Simulator Sickness Questionnaire (SSQ) (Kennedy, Lane, Berbaum, &amp; Lilienthal, 1993, cited in Steinicke &amp; Bruder 2014) has been used.</li>
<li>For measuring enjoyment of movement games, we might use: Physical Activity Enjoyment Scale (PACES) Kendzierski &amp; DeCarlo, 1991; cited in Mestre et. al., 2011)</li>
</ul>

<h2>Future Research</h2>
<p>The general direction for future research should be ‘generative design based work’, where artifacts are generated using user-centered design, and then evaluated and annotated with new theory. Gaver 2012) This would be an effective approach using the Oculus Rift and a stationary bicycle apparatus that was begun at NSU last year.  Given that a preliminary proof of concept / hardware demonstrator for going on a virtual reality bike ride is available from work initiated during coursework, the research should center on software game dynamics and game design elements.  Work might generally iterate around a cycle of; theory research,  user-centered artifact design and implementation, and the annotation of these artifacts with new theory.</p>
<p>Clearly more work needs to go into game enjoyment, game dynamics and what makes a good game.  These dynamics will also probably be generation, gender and personality specific to a a target audience (perhaps with older users preferring one level of stimuli, video game violence, problem solving, scenery and/or simulation, over younger users.)  Defining the target audience at an early stage would be important.</p>
<p>Brainstorming ideas in the design space at the concept level, one could imagine flight simulator game dynamics driven by bicycle cadence, first person shooter (FPS) dynamics where the virtual weapon, shields, or health regeneration was charged by exertion,  redirecting the flow of a particle stream by placing rigid bodies generated by pedaling in the stream…  Universal understanding of dynamics that might be applicable to many classes of exergames include, an understanding of how to handle scene reloading / character respawning to keep a rider moving,  what game dynamics could be placed in a 2 mile loop course to encourage the participant to cycle using interval intensity versus as a ‘long slow distance’ exercise… etc.</p>

<p>The human,  environmental and psychological contexts for increasingly urbanized users reflects a dire need for fun, motivating movement based experiences within a simultaneously hostile safety environment for riding outside for long distances.  Combined with newly relevant VR technology, understanding the relationship between apparatus, software, and participant in this domain should be a satisfying area for future exploration.</p>


<h2>References</h2>
<div style="font-family:sans-serif">
<p>Bolton, J., Lambert, M., Lirette, D., & Unsworth, B. (2014, April). PaperDude: a virtual reality cycling exergame. In CHI'14 Extended Abstracts on Human Factors in Computing Systems (pp. 475-478). ACM.</p>

<p>Buxton, B. (2010). Sketching user experiences: getting the design right and the right design: getting the design right and the right design. Morgan Kaufmann.</p>

<p>Carvalho, R. (2013). The magical features of immersive audiovisual environments. interactions, 20(5), 32-37.</p>

<p>Gaver, W. (2012, May). What should we expect from research through design?. In Proceedings of the SIGCHI conference on human factors in computing systems (pp. 937-946). ACM.</p>

<p>Huang, S., Tsai, P., Sung, W., Lin, C., & Chuang, T. (2008). The comparisons of heart rate variability and perceived exertion during simulated cycling with various viewing devices. Presence, 17(6), 575-583.</p>

<p>Kendzierski, D., & DeCarlo, K. J. (1991). Physical Activity Enjoyment Scale: Two validation studies. Journal of Sport & Exercise Psychology.</p>

<p>Kennedy, R. S., Lane, N. E., Berbaum, K. S., & Lilienthal, M. G. (1993). Simulator sickness questionnaire: An enhanced method for quantifying simulator sickness. The international journal of aviation psychology, 3(3), 203-220.</p>

<p>Mestre, D., Dagonneau, V., & Mercier, C. (2011). Does virtual reality enhance exercise performance, enjoyment, and dissociation? an exploratory study on a stationary bike apparatus. Presence, 20(1), 1-14.</p>

<p>Mueller, F., Gibbs, M. R., Vetere, F., & Edge, D. (2014, April). Supporting the creative game design process with exertion cards. In Proceedings of the 32nd annual ACM conference on Human factors in computing systems (pp. 2211-2220). ACM.</p>

<p>Mueller, F., & Isbister, K. (2014, April). Movement-based game guidelines. InProceedings of the 32nd annual ACM conference on Human factors in computing systems (pp. 2191-2200). ACM.</p>

<p>Park, T., Lee, U., MacKenzie, S., Moon, M., Hwang, I., & Song, J. (2014, April). Human factors of speed-based exergame controllers. In Proceedings of the 32nd annual ACM conference on Human factors in computing systems (pp. 1865-1874). ACM.</p>

<p>Steinicke, F., & Bruder, G. (2014, October). A self-experimentation report about long-term use of fully-immersive technology. In Proceedings of the 2nd ACM symposium on Spatial user interaction (pp. 66-69). ACM.</p>

<p>Usoh, M., Catena, E., Arman, S., & Slater, M. (2000). Using presence questionnaires in reality. Presence, 9(5), 497-503.</p>

<p>Weiser, M. (1994). The world is not a desktop. interactions, 1(1), 7-8.</p>

<p>Witmer, B. G., & Singer, M. J. (1998). Measuring presence in virtual environments: A presence questionnaire. Presence: Teleoperators and virtual environments, 7(3), 225-240.</p>

<p>Zhang, R., Li, B., & Kuhl, S. A. (2014, October). Human sensitivity to dynamic translational gains in head-mounted displays. In Proceedings of the 2nd ACM symposium on Spatial user interaction (pp. 62-65). ACM.</p>
</div>
