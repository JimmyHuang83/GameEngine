//LSNativeManager.h
 
#import <Foundation/Foundation.h>
 
@interface LSNativeManager : NSObject
 
+(LSNativeManager*)sharedInstance;
-(void)UIActivityVC:(NSString*)_text imgname:(NSString*)imgname;
 
@end
 