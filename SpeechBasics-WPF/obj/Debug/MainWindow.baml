   M S B A M L   `   `   `  ����  SpeechBasics-WPF    KinectChris.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  
MainWindow��5   	   $��723.135��6   $��929.478��6-   .��5      � .+�5   
   �� -   5         ��Name$ mostrarVideo��5      $
��Left=�6&   $	��333��6A   $��	29,40,0,0q�6N   $	��Top=�6a   $	��369��6y   �� 5   
   -   5         ��Name$ UP��5      $��UP��6   $
��Left=�6*   $��182,403,0,0q�6E   $	��Top=�6Z   $��75��6r   6}   �� 5   
   -   5        $
 DOWN��5      $
��DOWN��6   $
��Left=�6.   $��182,501,0,0q�6I   $	��Top=�6^   $��75��6v   6�   �� 5	   
   -   5        $
 REST��5	      $
��REST��6   $
��Left=�6.   $��182,627,0,0q�6I   $	��Top=�6^   $��75��6v   6�   �� 5
   
   -   5        $
 FOTO��5
      $
��FOTO��6   $
��Left=�6.   $��289,501,0,0q�6I   $	��Top=�6^   $��75��6v   6�   �� 5   
   -   5         ��Name$ 
sliderAzul��5       � Maximum$	 255Q�6%    � Minimum$
 -255Q�63    �� Orientation$ Vertical=�6B   
����  �$
��Left=�6k   $��
56,403,0,0q�6�   $	��Top=�6�   $	��266��6�   6�   �� 5   
   -   5        $ sliderVerde��5      $	 255Q�6&   $
 -255Q�64   $ Vertical=�6C   
��� � �$
��Left=�6m   $��
84,403,0,0q�6�   $	��Top=�6�   $	��266��6�   6�   �� 5   
   -	   5        $ 
sliderRojo��5      $	 255Q�6%   $
 -255Q�63   $ Vertical=�6B   
���  ��$
��Left=�6j   $��116,403,0,0q�6�   $	��Top=�6�   $	��266��6�   6�   �� 5   
   $��Center=�6   $��16(�61   $��315,625,202,33q�6?   $	��404��6W   Say: "�� 5      -
   5         �Name$ 	photoSpan��5      .�6*   Photo61   " , "�� 6<   -   5        $ upSpan��5   A   .�6O   UP6S   ", "�� 6]   -   5        $ downSpan��5   b   .�6r   Down6x   ", "�� 6�   -   5        $ restoreSpan��5   �   .�6�   	Restore6�   ", "�� 6�   -   5        $ faceSpan��5   �   .�6�   Face6�   "5      �� 5   
   -   5        $��	statusBar��5   K   $��3��6   $��23��6!   $��Stretch=�6-   $��Bottom=�6\   
�������.@�5      �� $��0q�6   .��5      �� -   5        $��statusBarText��5      63   5      5      �� 5   
   -   5         ��Name$ angule��5      $�� ��6    $
��Left=�6+   $��345,438,0,0q�6F   $	��Top=�6[   �� 5   
   -   5        	 ��Name$	 canvasEsqueleto��5      $
��Left=�6*   $	��480��6E   $��
438,42,0,0q�6R   $	��Top=�6f   $	��447��6~   
 J� RenderTransformOrigin$
 0.5,0.5&�6�   z�5      V� 5      .~�5      �� �� 5      �� 5       �� Angle$ -0.24Q�6&   T� 5      5      5       �� 5!   
   $��Angulo:��6   $
��Left=�6"   $��289,438,0,0q�6=   $	��Top=�6R   $
 -0.658,1.423&�6j   5#      5$      