   M S B A M L   `   `   `  ����C  ?RAPPTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:RAPPTestRAPPTest  5   7       RAPPTest.showcaseList   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   %myclr-namespace:RAPPTest     5   7     � Title$  showcaseList��6	   $	��300��6   $	��800��6+   
���   � � WindowStyle$ SingleBorderWindow=�6l    � Topmost . .��5      � .+�5   
   � $��1��6    � ColumnDefinitions 5      �� 5      $��* �6#   .+�5
      �� -   5        $��mediaListBox��5
       J� SnapsToDevicePixels . $��0,34,0,0q�6J   ��5      �� 5      .R�5      �  K� IsItemsHost . ��67   ��  �� Path$ ActualWidth� �� RelativeSource ��  �� Mode$ FindAncestor=�	 �� AncestorType"	 ��5      5      ��5      �� 5      .R�5      �� .+�5      �  5   "   �� 5   &   $
��160* �67   �� 5   &   $
��160* �67   �� 5   &   $
��160* �67   .+�5      � 
 � RowDefinitions
 5   "   �� 5   &   $	��80* �64   �� 5   &   $	��20* �64    5   "   �� 5    &   $	��240 �67   �� 5!   &   $	��240 �67   .+�5#   "   � $��0��6'   $��0��64   .+�5$   &   �� $��5,5,5,0q�6,   $	��100��6=   ��6J   �� $ FileName�5%   #   � 5&   "   $��1��6'   $��0��64   .+�5'   &   �� 
�������$��Center=�6^   $��Center=�6{   ��6,   �� $ Title�6{   5(   #   � 5)   "   $��1��6'   .+�5*   *   �� $
��Wrap=�64   
�������$
��Left=�6z   $��Center=�6�   ��6H   �� $ Sequence�6�   5+   #   5,      5-      5.      5/      51      52      �� 53   
   -   5        $��label1��53   d   $��Press ESC to exit��6   $��28��6,   $
��Left=�68   $��0,0,0,0q�6S   $	��Top=�6r   
������� J� Opacity$	 0.6Q�6�   $��14(�6�   54      55      