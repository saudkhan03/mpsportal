var adminmodule=angular.module("app-mps",[]);adminmodule.controller("admin-slabs",["$scope","$http",function(a,e){var n=[];a.wait=!0,e.get("/Admin/LoadSlabs").then(function(t){n=t.data,a.SlabList=n,a.wait=!1},function(t){alert(t.statusText),a.wait=!1}),a.changed=function(){a.SlabList=a.searchtext?performfilter(a.searchtext):n},performfilter=function(t){return filterby=t.toLocaleLowerCase(),n.filter(function(t){return-1!=t.slabName.toLocaleLowerCase().indexOf(filterby)})},a.getSlab=function(t){a.wait=!0,e.get("/Admin/getSlab",{params:{id:t}}).then(function(t){a.slab=t.data,a.wait=!1},function(t){a.wait=!1,alert(t.statusText)})},a.updateSlab=function(){a.wait=!0,e.post("/Admin/updateSlab",a.slab).then(function(t){a.wait=!1,alert("Expense Added/Updated"),window.location.reload(!0)},function(t){a.wait=!1,alert(t.statusText)})}}]);