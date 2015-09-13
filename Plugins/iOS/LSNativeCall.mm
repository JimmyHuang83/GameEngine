//LSNativeCall.mm
 
#import "LSNativeManager.h"
extern "C" {
 
    void UIActivityVC(char* text, char* imgname){
        [[LSNativeManager sharedInstance] UIActivityVC:[NSString stringWithUTF8String:text] imgname:[NSString stringWithUTF8String:imgname]];
    }
   
}