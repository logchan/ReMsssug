
def genInput(names, lsize):
    for name in names:
        namelow = name[0].lower() + name[1:]
        print('<div class="form-group row">')
        print(str.format('<label for="{0}" class="col-md-{2} control-label">{1}</label>', namelow, name, lsize))
        print('<div class="col-md-6">')
        print(str.format('<input name="{0}" id="{1}" type="text" class="form-control" disabled/>', name, namelow))
        print('</div>')
        print('</div>')
        
def genCheck(names, lsize):
    for name in names:
        namelow = name[0].lower() + name[1:]
        print('<div class="form-group row">')
        print(str.format('<label for="{0}" class="col-md-{2} control-label">{1}</label>', namelow, name, lsize))
        print('<div class="checkbox col-md-6">')
        print(str.format('<label for="{1}"><input type="checkbox" name="{0}" id="{1}" class="checkbox" value="true" checked disabled/>Description</label>', name, namelow))
        print('</div>')
        print('</div>')
        
