   M S B A M L   `   `   `  ����H  DCTTBOTTLINGQC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	     � Title$  Hennessy QC V1.0.0��5   	   $	��450��6$   $	��800��61   ��5
      �� 5   
    �� 
StartPoint$	 0,0&�6    �� EndPoint$	 1,1&�6/   .��5      �  � Color$ 	#FF007ACC��6    � Offset$	 0.0Q�6-   � 5      $ 	#FF4CAF50��6   $	 1.0Q�6-   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��4* �6   .+�5   
   ]� $��0��6   
�������.+�5      �� $	��Top=�6   5       �� 5#      $��About��6   $��40��6%   $��0q�61   $��Bottom=�6<   $��Stretch=�6W   $��Bottom=�6u   ��6�   �� *AboutCommand+5$      � 5'   
   $��1��6    � RowDefinitions 5(      �� 5)      $��* �6    �� 5+      $
��Auto �6     5.      �� 5/      $��1* �6#   �� 51      $��1* �6#   .+�56      � $��0��6   $��Center=�6#   $��Center=�6@   .+�57      A� $	��200��6   $	��200��6&   
��� � �$��5��6B   � 58      $��Center=�6   $��Center=�64   .+�59      �� $��36(�6B   $
��Bold"�6P   
���   �$��Center=�6u   $��Center=�6�   ��6    �� *GoodProductCount+6�   5:      5;      � 5>      $��1��6   $��Center=�6#   $��Center=�6@   .+�5?      A� $	��200��6   $	��200��6&   
���  ��$��5��6@   � 5@      $��Center=�6   $��Center=�64   .+�5A      �� $��36(�6A   $
��Bold"�6O   
���   �$��Center=�6t   $��Center=�6�   ��6    �� *BadProductCount+6�   5B      5C      �� 5F      $��1��6   $��2��6&    �� Orientation$ 
Horizontal=�6:   $��Center=�6S   $��Bottom=�6p   $��0,20,0,0q�6�   .+�5G      �� $	��100��6   $��30��6%   $
��10,0q�6~   ��61   �� *StartStopButtonText+��6Y   �� *StartStopCommand+6~   �� 5H      $��Reset��6   $	��100��6)   $��30��65   $
��10,0q�6b   ��6A   �� *ResetCommand+6b   5I      5J      5K      5L      