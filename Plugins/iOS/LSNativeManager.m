//LSNativeManager.m
 
#import "LSNativeManager.h"
 
@implementation LSNativeManager
 
static LSNativeManager *sharedInstance = nil;
 
-(void)UIActivityVC:(NSString*)_text imgname:(NSString*)imgname{
   
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *path = [paths objectAtIndex:0];
    NSString *imagePath = [path stringByAppendingPathComponent:imgname];
    
    NSData *data = [NSData dataWithContentsOfFile:imagePath];
    UIImage *image=[UIImage imageWithData:data];
    NSArray *postItems=@[_text,image];
    
    UIActivityViewController *controller = [[UIActivityViewController alloc] initWithActivityItems:postItems applicationActivities:nil];
    id rootVC = [[[[[UIApplication sharedApplication] keyWindow] subviews] objectAtIndex:0] nextResponder];
    //if iPhone
    if (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone)
    {
        [rootVC presentViewController:controller animated:YES completion:nil];
    }
    //if iPad
    else
    {
         UIView *view = [[[[UIApplication sharedApplication] keyWindow] subviews] objectAtIndex:0];
        // Change Rect to position Popover
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:controller];
        NSLog(@"%f",view.frame.size.width/2);
        [popup presentPopoverFromRect:CGRectMake(view.frame.size.width/2, view.frame.size.height/4, 0, 0)inView:view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    }
    
    
 /*
    NSArray *activityItems = @[_text];
       
    UIActivityViewController *activityVC = [[UIActivityViewController alloc]initWithActivityItems:activityItems applicationActivities:Nil];
    id rootVC = [[[[[UIApplication sharedApplication] keyWindow] subviews] objectAtIndex:0] nextResponder];
    [rootVC presentViewController:activityVC animated:TRUE completion:nil];
*/
}
 
#pragma mark Singleton Methods
+ (id)sharedInstance {
    @synchronized(self) {
        if(sharedInstance == nil)
            sharedInstance = [[super allocWithZone:NULL] init];
    }
    return sharedInstance;
}
 
+ (id)allocWithZone:(NSZone *)zone {
    return [[self sharedInstance] retain];
}
 
- (id)copyWithZone:(NSZone *)zone {
    return self;
}
 
- (id)retain {
    return self;
}
 
- (unsigned)retainCount {
    return UINT_MAX;
}
 
- (oneway void)release {
 
}
- (id)autorelease {
    return self;
}
- (id)init {
    if (self = [super init]) {
       
    }
    return self;
}
- (void)dealloc {
    [super dealloc];
}
 
@end