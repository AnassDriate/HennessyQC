   M S B A M L   `   `   `  ���� G  CCTTHennesyQC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1      WpfApp1.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:WpfApp1       � Title$  Hennessy QC V1.0.0��$	��450��$	��800������  �� 
StartPoint$	 0,0&� �� EndPoint$	 1,1&�.���  � Color$ 	#FF007ACC�� � Offset$	 0.0Q�� $ 	#FF4CAF50��$	 1.0Q� � 	Resources    GoodFlashAnimation �� TargetProperty �� From	 �� To	 g� Duration
 g� AutoReverse  BadFlashAnimation��%�   &	        &	 k     �� .���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ TransparentX�$ GreenX�$	 0:0:0.5D�
 . �� .���� $* $(Shape.Fill).(SolidColorBrush.Color)�$ TransparentX�$	 RedX�$	 0:0:0.5D�
 . .���  � ColumnDefinitions �� $��1* ��� $��4* �.+�]� $��0��
�������.+��� $	��Top=��� $��About��$��40��$��0q�$��Bottom=�$��Stretch=�$��Bottom=����� *AboutCommand+� $��1�� � RowDefinitions �� $��* ��� $
��Auto � �� $��1* ��� $��1* �.+�� $��0��$��Center=�$��Center=�.+�A� -    A�Name$ GoodEllipse��$	��200��$	��200��
��� � �$��5��
������ � $��Center=�$��Center=�.+��� $��36(�$
��Bold"�
���   �$��Center=�$��Center=����� *GoodProductCount+� $��1��$��Center=�$��Center=�.+�A� $	��200��$	��200��
���  ��$��5��� $��Center=�$��Center=�.+��� $��36(�$
��Bold"�
���   �$��Center=�$��Center=����� *BadProductCount+�� $��1��$��2�� �� Orientation$ 
Horizontal=�$��Center=�$��Bottom=�$��0,20,0,0q�.+��� $	��100��$��30��$
��10,0q����� *StartStopButtonText+���� *StartStopCommand+�� $��Reset��$	��100��$��30��$
��10,0q����� *ResetCommand+